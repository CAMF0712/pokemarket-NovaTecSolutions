using FuzzySharp;

namespace PokeGrading.Services
{
    public interface ISearchScoringService
    {
        SearchResult ScoreCandidates(
            IEnumerable<dynamic> candidates,
            string extractedText,
            CardAttributesFromOcr attributes);
    }

    public class SearchScoringService : ISearchScoringService
    {
        // Scoring parameters - centralized configuration
        private const int NameScoreWeight = 3;
        private const int CardNumberExactMatchScore = 250;
        private const int HpExactMatchScore = 150;
        private const int TypeExactMatchScore = 100;
        private const int MinimumCandidateScore = 60;
        private const int MaximumCandidateMatches = 10;
        private const int CandidateScoreWindow = 30;
        private const int MinimumBestMatchScore = 200;
        private const int FullConfidencePercentage = 100;
        private const int ConfidenceRoundingDecimals = 2;

        public SearchResult ScoreCandidates(
            IEnumerable<dynamic> candidates,
            string extractedText,
            CardAttributesFromOcr attributes)
        {
            var scoredCandidates = new List<ScoredCandidate>();

            foreach (var card in candidates)
            {
                int score = CalculateScore(card, extractedText, attributes);

                if (score >= MinimumCandidateScore)
                {
                    scoredCandidates.Add(new ScoredCandidate
                    {
                        Score = score,
                        Card = card
                    });
                }
            }

            // Sort by score descending and take top candidates
            var orderedCandidates = scoredCandidates
                .OrderByDescending(x => x.Score)
                .Take(MaximumCandidateMatches)
                .ToList();

            if (orderedCandidates.Count == 0)
            {
                return new SearchResult
                {
                    ScoredCandidates = orderedCandidates,
                    BestMatch = null,
                    Confidence = 0
                };
            }

            var bestMatch = orderedCandidates.First();

            // Filter candidates within score window of best match
            var filteredMatches = orderedCandidates
                .Where(x => bestMatch.Score - x.Score <= CandidateScoreWindow)
                .ToList();

            double confidence = CalculateConfidence(bestMatch, filteredMatches);

            return new SearchResult
            {
                ScoredCandidates = filteredMatches,
                BestMatch = bestMatch.Score >= MinimumBestMatchScore ? bestMatch : null,
                Confidence = confidence
            };
        }

        private int CalculateScore(
            dynamic card,
            string extractedText,
            CardAttributesFromOcr attributes)
        {
            int score = 0;

            string cardName = card.card_name?.ToString() ?? "";
            string cardNumber = card.card_number?.ToString() ?? "";
            string cardType = card.pokemon_type?.ToString() ?? "";
            int cardHp = card.hp ?? 0;

            // Name fuzzy matching
            int tokenScore = Fuzz.TokenSetRatio(extractedText, cardName);
            int partialScore = Fuzz.PartialRatio(extractedText, cardName);
            int nameScore = Math.Max(tokenScore, partialScore);
            score += nameScore * NameScoreWeight;

            // Set name fuzzy matching
            string setName = card.set_name?.ToString() ?? "";
            int setScore = Fuzz.PartialRatio(extractedText.ToLower(), setName.ToLower());
            score += setScore;

            // Exact matches from detected attributes
            if (!string.IsNullOrWhiteSpace(attributes.DetectedCardNumber) && cardNumber == attributes.DetectedCardNumber)
            {
                score += CardNumberExactMatchScore;
            }

            if (attributes.DetectedHp != null && cardHp == attributes.DetectedHp)
            {
                score += HpExactMatchScore;
            }

            if (attributes.DetectedType != null && cardType.Equals(attributes.DetectedType, StringComparison.OrdinalIgnoreCase))
            {
                score += TypeExactMatchScore;
            }

            return score;
        }

        private double CalculateConfidence(ScoredCandidate bestMatch, List<ScoredCandidate> filteredMatches)
        {
            if (filteredMatches.Count == 1)
            {
                return FullConfidencePercentage;
            }

            if (filteredMatches.Count >= 2)
            {
                var secondBest = filteredMatches.Skip(1).First();
                return Math.Round(
                    ((double)(bestMatch.Score - secondBest.Score) / bestMatch.Score) * FullConfidencePercentage,
                    ConfidenceRoundingDecimals);
            }

            return FullConfidencePercentage;
        }
    }
}

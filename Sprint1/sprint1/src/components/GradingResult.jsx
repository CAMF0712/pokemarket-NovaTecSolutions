import styles
    from "./GradingResult.module.css";

import { buildApiUrl }
    from "../config/api";

function GradingResult({ result }) {
    if (!result) {
        return null;
    }

    const confidence =
        Math.max(
            0,
            Math.min(
                result.confidence_score,
                100
            )
        );

    const confidenceColor =
        confidence >= 90
            ? styles.high
            : confidence >= 75
                ? styles.medium
                : styles.low;

    const grade =
        result.estimated_grade;

    const gradeStyle =
        grade >= 9.5
            ? styles.gradeGemMint
            : grade >= 9
                ? styles.gradeMint
                : grade >= 8
                    ? styles.gradeNearMint
                    : grade >= 7
                        ? styles.gradeExcellent
                        : styles.gradeGood;

    const gradeDescription =
        grade >= 9.5
            ? "Gem Mint"
            : grade >= 9
                ? "Mint"
                : grade >= 8
                    ? "Near Mint"
                    : grade >= 7
                        ? "Excellent"
                        : "Good";

    return (

        <div className={styles.container}>

            <div className={styles.slab}>

                <div className={styles.label}>

                    <div>

                        <h3>PokeGrading</h3>

                        <span>

                            {gradeDescription}

                        </span>

                    </div>

                    <div
                        className={
                            `${styles.gradeBadge} ${gradeStyle}`
                        }
                    >

                        {grade.toFixed(2)}

                    </div>

                </div>

                {
                    result.front_image_url &&

                    <div className={styles.imageContainer}>

                        <img

                            src={
                                buildApiUrl(
                                    result.front_image_url
                                )
                            }

                            alt="Card"

                            className={styles.cardImage}

                        />

                    </div>

                }

                <div className={styles.infoSection}>

                    <div className={styles.status}>

                        {result.status}

                    </div>

                    <div className={styles.confidenceSection}>

                        <div className={styles.confidenceHeader}>

                            <span>

                                Confidence

                            </span>

                            <span>

                                {confidence.toFixed(2)}%

                            </span>

                        </div>

                        <div className={styles.progressBar}>

                            <div

                                className={
                                    `${styles.progressFill} ${confidenceColor}`
                                }

                                style={{
                                    width:
                                        `${confidence}%`
                                }}

                            />

                        </div>

                    </div>

                    <div className={styles.metrics}>

                        <Metric
                            name="Centering"
                            value={result.centering}
                        />

                        <Metric
                            name="Corners"
                            value={result.corners}
                        />

                        <Metric
                            name="Edges"
                            value={result.edges}
                        />

                        <Metric
                            name="Surface"
                            value={result.surface}
                        />

                    </div>

                </div>

            </div>

        </div>

    );

}

function Metric({

    name,

    value

}) {

    return (

        <div className={styles.metricCard}>

            <span>

                {name}

            </span>

            <strong>

                {Number(value).toFixed(2)}

            </strong>

        </div>

    );

}

export default GradingResult;
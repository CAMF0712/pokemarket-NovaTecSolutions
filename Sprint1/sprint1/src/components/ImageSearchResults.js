import { buildApiUrl } from "../config/api";

export default function ImageSearchResults({
    searchResult,
    uploadedImage,
    clearSearch
}) {
    if (!searchResult) {
        return null;
    }

    return (
        <div
            className="
                border
                rounded
                p-4
                mb-4
            "
        >
            <div
                style={{
                    display: "flex",
                    gap: 20
                }}
            >
                <div>
                    <h4>
                        Imagen enviada
                    </h4>

                    <img
                        src={
                            URL.createObjectURL(
                                uploadedImage
                            )
                        }
                        alt=""
                        width={220}
                    />
                </div>

                <div>
                    <h4>
                        Coincidencias
                    </h4>

                    {
                        searchResult
                            .candidate_matches
                            ?.map(
                                item => (
                                    <div
                                        key={
                                            item.card
                                                .card_id
                                        }
                                    >
                                        <img
                                            src={
                                                buildApiUrl(item.card.image_url)
                                            }
                                            alt=""
                                            width={150}
                                        />

                                        <h5>
                                            {item.card.card_name}
                                        </h5>

                                        <br />

                                        Score:
                                        {
                                            item.score
                                        }

                                        <br />

                                        HP:
                                        {
                                            item.card
                                                .hp
                                        }

                                        <br />

                                        Set:
                                        {
                                            item.card
                                                .set_name
                                        }

                                        <hr />
                                    </div>
                                )
                            )
                    }

                    <button
                        onClick={
                            clearSearch
                        }
                    >
                        Limpiar búsqueda
                    </button>
                </div>
            </div>
        </div>
    );
}
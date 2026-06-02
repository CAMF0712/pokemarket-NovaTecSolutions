import { useEffect, useState } from "react";
import axios from "axios";

function CardCatalog({
    isAdmin = false,
    onEdit = null
}) {

    const [cards,
        setCards] = useState([]);

    const [loading,
        setLoading] = useState(true);

    useEffect(() => {

        loadCatalog();

    }, []);

    const loadCatalog = async () => {

        try {

            const response =
                await axios.get(
                    "https://localhost:7271/Card/catalog"
                );

            setCards(response.data);

        }
        catch (error) {

            console.error(error);

            alert(
                "Error loading catalog"
            );
        }
        finally {

            setLoading(false);
        }
    };

    if (loading) {

        return (
            <h3>
                Loading catalog...
            </h3>
        );
    }

    if (cards.length === 0) {

        return (
            <h3>
                No cards found
            </h3>
        );
    }

    return (

        <div>

            <h2>
                Card Catalog
            </h2>

            <div
                style={{
                    display: "grid",
                    gridTemplateColumns:
                        "repeat(auto-fill,minmax(280px,1fr))",
                    gap: "20px"
                }}
            >

                {
                    cards.map(card => (

                        <div
                            key={card.card_id}
                            style={{
                                border:
                                    "1px solid #ddd",
                                borderRadius:
                                    "10px",
                                padding:
                                    "15px",
                                background:
                                    "#fff"
                            }}
                        >

                            {
                                card.image_url &&
                                (
                                    <img
                                        src={
                                            `https://localhost:7271${card.image_url}`
                                        }
                                        alt={
                                            card.card_name
                                        }
                                        style={{
                                            width:
                                                "100%",
                                            height:
                                                "350px",
                                            objectFit:
                                                "contain",
                                            marginBottom:
                                                "10px"
                                        }}
                                    />
                                )
                            }

                            <h3>
                                {card.card_name}
                            </h3>

                            <p>
                                <strong>
                                    Set:
                                </strong>
                                {" "}
                                {card.set_name}
                            </p>

                            <p>
                                <strong>
                                    Number:
                                </strong>
                                {" "}
                                {card.card_number}
                            </p>

                            <p>
                                <strong>
                                    Rarity:
                                </strong>
                                {" "}
                                {card.rarity}
                            </p>

                            <p>
                                <strong>
                                    Type:
                                </strong>
                                {" "}
                                {card.pokemon_type}
                            </p>

                            <p>
                                <strong>
                                    HP:
                                </strong>
                                {" "}
                                {card.hp}
                            </p>

                            {
                                isAdmin &&
                                onEdit &&
                                (
                                    <button
                                        onClick={() =>
                                            onEdit(card)
                                        }
                                        style={{
                                            width:
                                                "100%",
                                            padding:
                                                "10px",
                                            marginTop:
                                                "10px"
                                        }}
                                    >
                                        Edit Card
                                    </button>
                                )
                            }

                        </div>
                    ))
                }

            </div>

        </div>
    );
}

export default CardCatalog;
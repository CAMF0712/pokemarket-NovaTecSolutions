import { useEffect, useState } from "react";
import axios from "axios";

function CardCatalog() {

    const [cards, setCards] =
        useState([]);

    const loadCards = async () => {

        try {

            const response =
                await axios.get(
                    "https://localhost:7271/Card/catalog"
                );

            setCards(
                response.data
            );
        }
        catch {

            console.log(
                "Error loading cards"
            );
        }
    };

    useEffect(() => {

        loadCards();

    }, []);

    return (

        <div>

            <h2>
                Card Catalog
            </h2>

            <div
                style={{
                    display: "grid",
                    gridTemplateColumns:
                        "repeat(auto-fill,minmax(250px,1fr))",
                    gap: "20px"
                }}
            >

                {cards.map(card => (

                    <div
                        key={card.card_id}
                        style={{
                            border: "1px solid #ddd",
                            borderRadius: "10px",
                            padding: "15px",
                            background: "#fff"
                        }}
                    >

                        {card.image_url && (

                            <img
                                src={
                                    "https://localhost:7271" +
                                    card.image_url
                                }
                                alt={
                                    card.card_name
                                }
                                style={{
                                    width: "100%",
                                    borderRadius: "10px"
                                }}
                            />

                        )}

                        <h4>
                            {card.card_name}
                        </h4>

                        <p>
                            <b>Set:</b>
                            {" "}
                            {card.set_name}
                        </p>

                        <p>
                            <b>Number:</b>
                            {" "}
                            {card.card_number}
                        </p>

                        <p>
                            <b>Type:</b>
                            {" "}
                            {card.pokemon_type}
                        </p>

                        <p>
                            <b>HP:</b>
                            {" "}
                            {card.hp}
                        </p>

                        <p>
                            <b>Rarity:</b>
                            {" "}
                            {card.rarity}
                        </p>

                    </div>

                ))}

            </div>

        </div>
    );
}

export default CardCatalog;
import { useState } from "react";
import axios from "axios";

function AdminPg() {

    const user =
        JSON.parse(
            localStorage.getItem(
                "usuario_actual"
            )
        );

    const [form, setForm] = useState({
        card_name: "",
        set_name: "",
        card_number: "",
        edition: "",
        language: "",
        finish_type: "",
        rarity: "",
        pokemon_type: "",
        hp: "",
        illustrator: "",
        release_year: ""
    });

    const [frontImage, setFrontImage] =
        useState(null);

    const [backImage, setBackImage] =
        useState(null);

    //----------------------------------
    // Inputs
    //----------------------------------

    const handleChange = (e) => {

        setForm({
            ...form,
            [e.target.name]:
                e.target.value
        });
    };

    //----------------------------------
    // Submit
    //----------------------------------

    const addCard = async (e) => {

        e.preventDefault();

        try {

            const formData =
                new FormData();

            formData.append(
                "created_by",
                user.user_id
            );

            Object.keys(form).forEach(
                key => {

                    formData.append(
                        key,
                        form[key]
                    );
                }
            );

            if (frontImage) {

                formData.append(
                    "front_image",
                    frontImage
                );
            }

            if (backImage) {

                formData.append(
                    "back_image",
                    backImage
                );
            }

            const response =
                await axios.post(
                    "https://localhost:7271/Card/create",
                    formData,
                    {
                        headers: {
                            "Content-Type":
                                "multipart/form-data"
                        }
                    }
                );

            alert(
                `Card created!\n\nCard ID: ${response.data.card_id}`
            );

        }
        catch (error) {

            console.error(error);

            alert(
                error.response?.data ||
                "Error creating card"
            );
        }
    };

    return (

        <div
            style={{
                maxWidth: "1000px",
                margin: "40px auto",
                padding: "30px",
                borderRadius: "12px",
                background: "#ffffff",
                boxShadow:
                    "0px 4px 12px rgba(0,0,0,0.15)"
            }}
        >

            <h1
                style={{
                    textAlign: "center",
                    marginBottom: "30px"
                }}
            >
                Pokémon Card Administration
            </h1>

            <form onSubmit={addCard}>

                <div
                    style={{
                        display: "grid",
                        gridTemplateColumns:
                            "1fr 1fr",
                        gap: "15px"
                    }}
                >

                    <input
                        name="card_name"
                        placeholder="Card Name"
                        value={form.card_name}
                        onChange={handleChange}
                    />

                    <input
                        name="set_name"
                        placeholder="Set Name"
                        value={form.set_name}
                        onChange={handleChange}
                    />

                    <input
                        name="card_number"
                        placeholder="Card Number"
                        value={form.card_number}
                        onChange={handleChange}
                    />

                    <select
                        name="edition"
                        value={form.edition}
                        onChange={handleChange}
                    >
                        <option value="">
                            Select Edition
                        </option>

                        <option value="1st Edition">
                            1st Edition
                        </option>

                        <option value="Unlimited">
                            Unlimited
                        </option>

                        <option value="Shadowless">
                            Shadowless
                        </option>
                    </select>

                    <select
                        name="language"
                        value={form.language}
                        onChange={handleChange}
                    >
                        <option value="">
                            Select Language
                        </option>

                        <option value="English">
                            English
                        </option>

                        <option value="Spanish">
                            Spanish
                        </option>

                        <option value="Japanese">
                            Japanese
                        </option>

                        <option value="German">
                            German
                        </option>

                        <option value="French">
                            French
                        </option>

                        <option value="Italian">
                            Italian
                        </option>

                        <option value="Portuguese">
                            Portuguese
                        </option>
                    </select>

                    <select
                        name="finish_type"
                        value={form.finish_type}
                        onChange={handleChange}
                    >
                        <option value="">
                            Select Finish Type
                        </option>

                        <option value="Non-Holo">
                            Non-Holo
                        </option>

                        <option value="Holo">
                            Holo
                        </option>

                        <option value="Reverse Holo">
                            Reverse Holo
                        </option>

                        <option value="Full Art">
                            Full Art
                        </option>
                    </select>

                    <select
                        name="rarity"
                        value={form.rarity}
                        onChange={handleChange}
                    >
                        <option value="">
                            Select Rarity
                        </option>

                        <option value="Common">
                            Common
                        </option>

                        <option value="Uncommon">
                            Uncommon
                        </option>

                        <option value="Rare">
                            Rare
                        </option>

                        <option value="Holo Rare">
                            Holo Rare
                        </option>

                        <option value="Ultra Rare">
                            Ultra Rare
                        </option>

                        <option value="Secret Rare">
                            Secret Rare
                        </option>
                    </select>

                    <select
                        name="pokemon_type"
                        value={form.pokemon_type}
                        onChange={handleChange}
                    >
                        <option value="">
                            Select Pokémon Type
                        </option>

                        <option value="Grass">
                            Grass
                        </option>

                        <option value="Fire">
                            Fire
                        </option>

                        <option value="Water">
                            Water
                        </option>

                        <option value="Lightning">
                            Lightning
                        </option>

                        <option value="Psychic">
                            Psychic
                        </option>

                        <option value="Fighting">
                            Fighting
                        </option>

                        <option value="Darkness">
                            Darkness
                        </option>

                        <option value="Metal">
                            Metal
                        </option>

                        <option value="Dragon">
                            Dragon
                        </option>

                        <option value="Fairy">
                            Fairy
                        </option>

                        <option value="Colorless">
                            Colorless
                        </option>
                    </select>

                    <input
                        type="number"
                        min="1"
                        max="500"
                        name="hp"
                        placeholder="HP"
                        value={form.hp}
                        onChange={handleChange}
                    />

                    <input
                        name="illustrator"
                        placeholder="Illustrator"
                        value={form.illustrator}
                        onChange={handleChange}
                    />

                    <input
                        type="number"
                        min="1996"
                        max={new Date().getFullYear()}
                        name="release_year"
                        placeholder="Release Year"
                        value={form.release_year}
                        onChange={handleChange}
                    />

                </div>

                <hr
                    style={{
                        margin:
                            "30px 0"
                    }}
                />

                <h3>
                    Images
                </h3>

                <div
                    style={{
                        marginBottom: "20px"
                    }}
                >
                    <label>
                        Front Image (Required)
                    </label>

                    <br />

                    <input
                        type="file"
                        accept="image/*"
                        onChange={(e) =>
                            setFrontImage(
                                e.target.files[0]
                            )
                        }
                    />
                </div>

                <div
                    style={{
                        marginBottom: "20px"
                    }}
                >
                    <label>
                        Back Image (Optional)
                    </label>

                    <br />

                    <input
                        type="file"
                        accept="image/*"
                        onChange={(e) =>
                            setBackImage(
                                e.target.files[0]
                            )
                        }
                    />
                </div>

                {frontImage && (

                    <div
                        style={{
                            marginBottom: "20px"
                        }}
                    >
                        <h4>
                            Front Preview
                        </h4>

                        <img
                            src={
                                URL.createObjectURL(
                                    frontImage
                                )
                            }
                            alt="Front Preview"
                            style={{
                                width: "250px",
                                borderRadius: "10px"
                            }}
                        />
                    </div>
                )}

                <button
                    type="submit"
                    style={{
                        width: "100%",
                        padding: "15px",
                        fontSize: "16px",
                        fontWeight: "bold",
                        border: "none",
                        borderRadius: "8px",
                        cursor: "pointer"
                    }}
                >
                    Create Card
                </button>

            </form>

        </div>
    );
}

export default AdminPg;
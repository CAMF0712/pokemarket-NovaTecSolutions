import { useState } from "react";
import axios from "axios";
import CardCatalog
    from "../components/CardCatalog";

function AdminPg() {

    const user =
        JSON.parse(
            localStorage.getItem(
                "usuario_actual"
            )
        );

    const [refreshKey,
        setRefreshKey] = useState(0);

    const initialForm = {

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
        release_year: "",

        front_image: null,
        back_image: null
    };

    const [form, setForm] =
        useState(initialForm);

    //----------------------------------
    // Text / Select Changes
    //----------------------------------

    const handleChange = (e) => {

        setForm({
            ...form,
            [e.target.name]:
                e.target.value
        });
    };

    //----------------------------------
    // File Changes
    //----------------------------------

    const handleFileChange =
        (e) => {

            setForm({
                ...form,
                [e.target.name]:
                    e.target.files[0]
            });
        };

    //----------------------------------
    // Create Card
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

            Object.keys(form)
                .forEach(key => {

                    if (
                        form[key] !== null &&
                        form[key] !== ""
                    ) {
                        formData.append(
                            key,
                            form[key]
                        );
                    }
                });

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
                "Card created successfully"
            );

            setRefreshKey(
                previous =>
                    previous + 1
            );

            //----------------------------------
            // Reset Form
            //----------------------------------

            setForm(initialForm);

            const frontInput =
                document.getElementById(
                    "front_image"
                );

            const backInput =
                document.getElementById(
                    "back_image"
                );

            if (frontInput)
                frontInput.value = "";

            if (backInput)
                backInput.value = "";

        }
        catch (error) {

            alert(
                error.response?.data ||
                "Error creating card"
            );
        }
    };

    return (

        <div
            style={{
                maxWidth: "900px",
                margin: "40px auto",
                padding: "30px",
                border: "1px solid #ddd",
                borderRadius: "10px"
            }}
        >

            <h1>
                Add Pokémon Card
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

                    <input
                        name="hp"
                        type="number"
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
                        name="release_year"
                        type="number"
                        placeholder="Release Year"
                        value={form.release_year}
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
                        <option>
                            Unlimited
                        </option>
                        <option>
                            1st Edition
                        </option>
                        <option>
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
                        <option>
                            English
                        </option>
                        <option>
                            Spanish
                        </option>
                        <option>
                            Japanese
                        </option>
                        <option>
                            German
                        </option>
                        <option>
                            French
                        </option>
                        <option>
                            Italian
                        </option>
                        <option>
                            Portuguese
                        </option>
                    </select>

                    <select
                        name="finish_type"
                        value={form.finish_type}
                        onChange={handleChange}
                    >
                        <option value="">
                            Select Finish
                        </option>
                        <option>
                            Non-Holo
                        </option>
                        <option>
                            Holo
                        </option>
                        <option>
                            Reverse Holo
                        </option>
                        <option>
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
                        <option>
                            Common
                        </option>
                        <option>
                            Uncommon
                        </option>
                        <option>
                            Rare
                        </option>
                        <option>
                            Holo Rare
                        </option>
                        <option>
                            Ultra Rare
                        </option>
                        <option>
                            Secret Rare
                        </option>
                    </select>

                    <select
                        name="pokemon_type"
                        value={form.pokemon_type}
                        onChange={handleChange}
                    >
                        <option value="">
                            Select Type
                        </option>
                        <option>
                            Grass
                        </option>
                        <option>
                            Fire
                        </option>
                        <option>
                            Water
                        </option>
                        <option>
                            Lightning
                        </option>
                        <option>
                            Psychic
                        </option>
                        <option>
                            Fighting
                        </option>
                        <option>
                            Darkness
                        </option>
                        <option>
                            Metal
                        </option>
                        <option>
                            Dragon
                        </option>
                        <option>
                            Fairy
                        </option>
                        <option>
                            Colorless
                        </option>
                    </select>

                </div>

                <hr />

                <div
                    style={{
                        marginBottom: "15px"
                    }}
                >
                    <label>
                        Front Image
                        (Required)
                    </label>

                    <br />

                    <input
                        id="front_image"
                        name="front_image"
                        type="file"
                        accept="image/*"
                        onChange={
                            handleFileChange
                        }
                    />
                </div>

                <div
                    style={{
                        marginBottom: "15px"
                    }}
                >
                    <label>
                        Back Image
                        (Optional)
                    </label>

                    <br />

                    <input
                        id="back_image"
                        name="back_image"
                        type="file"
                        accept="image/*"
                        onChange={
                            handleFileChange
                        }
                    />
                </div>

                <button
                    type="submit"
                    style={{
                        padding:
                            "10px 20px",
                        fontSize: "16px"
                    }}
                >
                    Create Card
                </button>

            </form>

            <hr />

            <CardCatalog
                key={refreshKey}
            />

        </div>
    );
}

export default AdminPg;
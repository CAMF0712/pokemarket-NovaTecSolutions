import { useState } from "react";
import styles from "./AdminPg.module.css";
import axios from "axios";
import CardCatalog
    from "../components/CardCatalog";
import { buildApiUrl } from "../config/api";
import {useNavigate} from "react-router-dom";

function AdminPg() {

    const user =
        JSON.parse(
            localStorage.getItem(
                "usuario_actual"
            )
        );

    const [refreshKey,
        setRefreshKey] = useState(0);

    const [editingCard,
        setEditingCard] =
        useState(null);

    const [isEditing,
        setIsEditing] =
        useState(false);

    const navigate =
        useNavigate();

    const logout = () => {

        localStorage.removeItem(
            "usuario_actual"
        );

        navigate(
            "/login",
            {
                replace: true
            }
        );
    };


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
    // Load card into editor
    //----------------------------------

    const loadCardForEdit =
        (card) => {

            setForm({

                card_name:
                    card.card_name || "",

                set_name:
                    card.set_name || "",

                card_number:
                    card.card_number || "",

                edition:
                    card.edition || "",

                language:
                    card.language || "",

                finish_type:
                    card.finish_type || "",

                rarity:
                    card.rarity || "",

                pokemon_type:
                    card.pokemon_type || "",

                hp:
                    card.hp || "",

                illustrator:
                    card.illustrator || "",

                release_year:
                    card.release_year || "",

                front_image: null,
                back_image: null
            });

            setEditingCard(card);

            setIsEditing(true);

            window.scrollTo({
                top: 0,
                behavior: "smooth"
            });
        };

    //----------------------------------
    // Create / Edit Card
    //----------------------------------

    const addCard = async (e) => {

        e.preventDefault();

        try {

            //----------------------------------
            // EDIT MODE
            //----------------------------------

            if (isEditing) {

                await axios.post(
                    buildApiUrl("/Card/version"),
                    {
                        card_id:
                            editingCard.card_id,

                        created_by:
                            user.user_id,

                        ...form,

                        hp:
                            parseInt(
                                form.hp
                            ),

                        release_year:
                            parseInt(
                                form.release_year
                            )
                    }
                );

                alert(
                    "New version created"
                );
            }

            //----------------------------------
            // CREATE MODE
            //----------------------------------

            else {

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
                    buildApiUrl("/Card/create"),
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
            }

            //----------------------------------
            // Refresh catalog
            //----------------------------------

            setRefreshKey(
                previous =>
                    previous + 1
            );

            //----------------------------------
            // Reset form
            //----------------------------------

            setForm(initialForm);

            setEditingCard(null);

            setIsEditing(false);

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
                "Operation failed"
            );
        }
    };

    return (

        <div className={styles.adminContainer}>

            <button
                type="button"
                className={styles.logoutButton}
                onClick={logout}
            >
                Logout
            </button>

            <h1 className={styles.title}>
                {
                    isEditing
                        ? "Edit Pokémon Card"
                        : "Add Pokémon Card"
                }
            </h1>

            <form onSubmit={addCard}>

                <div className={styles.formGrid}>

                    <input
                        className={styles.input}
                        name="card_name"
                        placeholder="Card Name"
                        value={form.card_name}
                        onChange={handleChange}
                    />

                    <input
                        className={styles.input}
                        name="set_name"
                        placeholder="Set Name"
                        value={form.set_name}
                        onChange={handleChange}
                    />

                    <input
                        className={styles.input}
                        name="card_number"
                        placeholder="Card Number"
                        value={form.card_number}
                        onChange={handleChange}
                    />

                    <input
                        className={styles.input}
                        name="hp"
                        type="number"
                        placeholder="HP"
                        value={form.hp}
                        onChange={handleChange}
                    />

                    <input
                        className={styles.input}
                        name="illustrator"
                        placeholder="Illustrator"
                        value={form.illustrator}
                        onChange={handleChange}
                    />

                    <input
                        className={styles.input}
                        name="release_year"
                        type="number"
                        placeholder="Release Year"
                        value={form.release_year}
                        onChange={handleChange}
                    />

                    <select
                        className={styles.select}
                        name="edition"
                        value={form.edition}
                        onChange={handleChange}
                    >
                        <option value="">
                            Select Edition
                        </option>
                        <option>Unlimited</option>
                        <option>1st Edition</option>
                        <option>Shadowless</option>
                    </select>

                    <select
                        className={styles.select}
                        name="language"
                        value={form.language}
                        onChange={handleChange}
                    >
                        <option value="">
                            Select Language
                        </option>
                        <option>English</option>
                        <option>Spanish</option>
                        <option>Japanese</option>
                        <option>German</option>
                        <option>French</option>
                        <option>Italian</option>
                        <option>Portuguese</option>
                    </select>

                    <select
                        className={styles.select}
                        name="finish_type"
                        value={form.finish_type}
                        onChange={handleChange}
                    >
                        <option value="">
                            Select Finish
                        </option>
                        <option>Non-Holo</option>
                        <option>Holo</option>
                        <option>Reverse Holo</option>
                        <option>Full Art</option>
                    </select>

                    <select
                        className={styles.select}
                        name="rarity"
                        value={form.rarity}
                        onChange={handleChange}
                    >
                        <option value="">
                            Select Rarity
                        </option>
                        <option>Common</option>
                        <option>Uncommon</option>
                        <option>Rare</option>
                        <option>Holo Rare</option>
                        <option>Ultra Rare</option>
                        <option>Secret Rare</option>
                    </select>

                    <select
                        className={styles.select}
                        name="pokemon_type"
                        value={form.pokemon_type}
                        onChange={handleChange}
                    >
                        <option value="">
                            Select Type
                        </option>
                        <option>Grass</option>
                        <option>Fire</option>
                        <option>Water</option>
                        <option>Lightning</option>
                        <option>Psychic</option>
                        <option>Fighting</option>
                        <option>Darkness</option>
                        <option>Metal</option>
                        <option>Dragon</option>
                        <option>Fairy</option>
                        <option>Colorless</option>
                    </select>

                </div>

                <hr className={styles.divider} />

                {
                    !isEditing &&
                    <>
                        <div className={styles.fileSection}>
                            <label className={styles.fileLabel}>
                                Front Image (Required)
                            </label>

                            <input
                                id="front_image"
                                name="front_image"
                                type="file"
                                accept="image/*"
                                onChange={handleFileChange}
                            />
                        </div>

                        <div className={styles.fileSection}>
                            <label className={styles.fileLabel}>
                                Back Image (Optional)
                            </label>

                            <input
                                id="back_image"
                                name="back_image"
                                type="file"
                                accept="image/*"
                                onChange={handleFileChange}
                            />
                        </div>
                    </>
                }

                <div className={styles.actions}>

                    <button
                        type="submit"
                        className={styles.button}
                    >
                        {
                            isEditing
                                ? "Save Version"
                                : "Create Card"
                        }
                    </button>

                    {
                        isEditing &&
                        (
                            <button
                                type="button"
                                className={styles.cancelButton}
                                onClick={() => {

                                    setForm(initialForm);

                                    setEditingCard(null);

                                    setIsEditing(false);
                                }}
                            >
                                Cancel
                            </button>
                        )
                    }

                </div>

            </form>

            <hr className={styles.divider} />

            <div className={styles.catalogSection}>
                <CardCatalog
                    key={refreshKey}
                    isAdmin={true}
                    onEdit={loadCardForEdit}
                />
            </div>
        </div>
    );
}

export default AdminPg;
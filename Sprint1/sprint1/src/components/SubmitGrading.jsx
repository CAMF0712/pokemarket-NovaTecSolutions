import { useState }
    from "react";

import axios
    from "axios";

import styles
    from "./SubmitGrading.module.css";

import { buildApiUrl }
    from "../config/api";

import GradingResult
    from "./GradingResult";

function SubmitGrading({selectedCard}) {


    const [frontImage, setFrontImage] =
        useState(null);

    const [backImage, setBackImage] =
        useState(null);

    const [loading, setLoading] =
        useState(false);

    const [gradingResult,
        setGradingResult] =
        useState(null);

    const handleSubmit =
        async (e) => {

            e.preventDefault();

            const usuario =
                JSON.parse(
                    localStorage.getItem(
                        "usuario_actual"
                    )
                );

            if (!usuario) {

                alert(
                    "Debe iniciar sesión"
                );

                return;
            }

            if (!selectedCard)
            {
                alert(
                    "Debe seleccionar una carta del catálogo."
                );

                return;
            }

            if (!frontImage) {

                alert(
                    "Debe adjuntar imagen frontal"
                );

                return;
            }

            try {

                setLoading(true);

                setGradingResult(null);

                const formData =
                    new FormData();

                formData.append(
                    "user_id",
                    usuario.user_id
                );

                formData.append(
                    "card_id",
                    selectedCard.card_id
                );

                formData.append(
                    "front_image",
                    frontImage
                );

                if (backImage) {

                    formData.append(
                        "back_image",
                        backImage
                    );
                }

                const response =
                    await axios.post(
                        buildApiUrl("/Grading/submit"),
                        formData,
                        {
                            headers: {
                                "Content-Type":
                                    "multipart/form-data"
                            }
                        }
                    );

                const result =
                    response.data.data;

                setGradingResult(
                    result
                );

                setFrontImage(null);
                setBackImage(null);

            }
            catch {

                alert(
                    "Error enviando grading"
                );
            }
            finally {

                setLoading(false);
            }
        };

    return (

        <div
            className={
                styles.container
            }
        >

            <h2>
                Submit Card For Grading
            </h2>

            <form
                onSubmit={
                    handleSubmit
                }
            >

                {
                    selectedCard &&
                    (
                        <div
                            className={
                                styles.cardInfo
                            }
                        >

                            <h3>
                                Selected Card
                            </h3>

                            <p>
                                <strong>
                                    {selectedCard.card_name}
                                </strong>
                            </p>

                            <p>
                                Set:
                                {" "}
                                {selectedCard.set_name}
                            </p>

                            <p>
                                Number:
                                {" "}
                                {selectedCard.card_number}
                            </p>

                            <p
                                className={
                                    styles.infoText
                                }
                            >
                                Please upload photos of your
                                own physical card for
                                evaluation. The grading
                                process analyzes the images
                                you provide, not the catalog
                                reference image.
                            </p>

                        </div>
                    )
                }

                <div
                    className={
                        styles.fileGroup
                    }
                >

                    <label>
                        Front Image
                    </label>

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
                    className={
                        styles.fileGroup
                    }
                >

                    <label>
                        Back Image
                    </label>

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

                <button
                    type="submit"
                    className={
                        styles.button
                    }
                    disabled={
                        loading ||
                        !selectedCard
                    }
                >

                    {
                        loading
                            ? "Sending..."
                            : "Submit Grading"
                    }

                </button>

            </form>

            <GradingResult
                result={
                    gradingResult
                }
            />

        </div>
    );
}

export default SubmitGrading;
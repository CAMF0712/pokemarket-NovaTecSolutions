import { useState } from "react";

export default function ImageSearch({
    onResults
}) {
    const [preview, setPreview] =
        useState(null);

    const [loading, setLoading] =
        useState(false);

    const handleUpload = async (e) => {
        const file =
            e.target.files?.[0];

        if (!file) return;

        setPreview(
            URL.createObjectURL(file)
        );

        const formData =
            new FormData();

        formData.append(
            "image",
            file
        );

        setLoading(true);

        try {
            const response =
                await fetch(
                    "https://localhost:7271/Card/search-by-image",
                    {
                        method: "POST",
                        body: formData
                    }
                );

            const data =
                await response.json();

            onResults(
                file,
                data
            );
        }
        finally {
            setLoading(false);
        }
    };

    return (
        <div
            className="
                border
                rounded
                p-3
                mb-4
            "
        >
            <h3>
                Buscar carta por imagen
            </h3>

            <input
                type="file"
                accept="image/*"
                onChange={handleUpload}
            />

            {
                loading &&
                <p>
                    Analizando imagen...
                </p>
            }

            {
                preview &&
                (
                    <img
                        src={preview}
                        alt=""
                        style={{
                            width: 200,
                            marginTop: 10
                        }}
                    />
                )
            }
        </div>
    );
}
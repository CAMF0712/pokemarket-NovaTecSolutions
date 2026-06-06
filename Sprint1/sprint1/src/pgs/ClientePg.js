import CardCatalog
    from "../components/CardCatalog";

import SubmitGrading
    from "../components/SubmitGrading";

import styles
    from "./ClientePg.module.css";
import {useState} from "react";

function ClientePg() {
    const [selectedCard,
        setSelectedCard] = useState(null);

    return (

        <div
            className={
                styles.page
            }
        >

            <h1
                className={
                    styles.title
                }
            >
                Pokémon Catalog
            </h1>

            <SubmitGrading
                selectedCard={selectedCard}
            />

            <CardCatalog
                onEvaluate={setSelectedCard}
            />

        </div>

    );
}

export default ClientePg;
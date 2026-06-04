import CardCatalog
    from "../components/CardCatalog";

import SubmitGrading
    from "../components/SubmitGrading";

import styles
    from "./ClientePg.module.css";

function ClientePg() {

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

            <SubmitGrading />

            <CardCatalog />

        </div>

    );
}

export default ClientePg;
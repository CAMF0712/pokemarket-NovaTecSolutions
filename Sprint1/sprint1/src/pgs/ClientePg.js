import CardCatalog
    from "../components/CardCatalog";

import SubmitGrading
    from "../components/SubmitGrading";

import styles
    from "./ClientePg.module.css";

import {
    useState
} from "react";

import {
    useNavigate
} from "react-router-dom";

function ClientePg() {

    const [
        selectedCard,
        setSelectedCard
    ] = useState(null);

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

    return (

        <div
            className={
                styles.page
            }
        >

            <div
                className={
                    styles.header
                }
            >

                <h1
                    className={
                        styles.title
                    }
                >
                    Pokémon Catalog
                </h1>

                <button
                    className={
                        styles.logoutButton
                    }
                    onClick={
                        logout
                    }
                >
                    Logout
                </button>

            </div>

            <SubmitGrading
                selectedCard={
                    selectedCard
                }
            />

            <CardCatalog
                onEvaluate={
                    setSelectedCard
                }
            />

        </div>
    );
}

export default ClientePg;
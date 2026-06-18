import {
    BrowserRouter as Router,
    Routes,
    Route,
    Navigate
} from "react-router-dom";

import { useState, useEffect } from "react";

import Login from "./pgs/Login";
import AdminPg from "./pgs/AdminPg";
import ClientePg from "./pgs/ClientePg";

import 'bootstrap/dist/css/bootstrap.min.css';

function App() {

    const [user, setUser] = useState(null);

    const resolvedUser =
        user ||
        JSON.parse(
            localStorage.getItem("usuario_actual") ||
            "null"
        );

    useEffect(() => {

        const savedUser =
            localStorage.getItem("usuario_actual");

        if (savedUser) {
            setUser(JSON.parse(savedUser));
        }

    }, []);

    return (

        <Router>

            <Routes>

                <Route
                    path="/login"
                    element={
                        <Login setUser={setUser} />
                    }
                />

                <Route
                    path="/admin"
                    element={
                        resolvedUser?.role === "ADMIN"
                            ? <AdminPg />
                            : <Navigate to="/login" />
                    }
                />

                <Route
                    path="/catalog"
                    element={
                        resolvedUser?.role === "SUBMITTER"
                            ? <ClientePg />
                            : <Navigate to="/login" />
                    }
                />

                <Route
                    path="*"
                    element={
                        <Navigate to="/login" />
                    }
                />

            </Routes>

        </Router>

    );
}

export default App;
import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { FaRegUser } from "react-icons/fa";
import { MdLockOutline } from "react-icons/md";
import React from "react";
import axios from "axios";
import styles from './Login.module.css';

function Login({ setUser }) {
    const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");
    const [cuenta, setCuenta] = useState(null);
    const [showPassword, setShowPassword] = useState(false);
    const navigate = useNavigate();

    useEffect(() => {
        const cuentaGuardada = JSON.parse(localStorage.getItem("cuenta_actual"));
        if (cuentaGuardada) {
            setCuenta(cuentaGuardada);
        }
    }, []);

    const handleLogin = async (e) => {
        e.preventDefault();

        // Logins locales de prueba
        if (username === "admin" && password === "admin123") {
            const adminUser = {
                nombre: "Admin",
                usuario: "admin",
                contrasena: "admin123",
                rol: "Admin"
            };
            setUser(adminUser);
            localStorage.setItem("usuario_actual", JSON.stringify(adminUser));
            navigate("/admin");
            return;
        }

        if (username === "cliente" && password === "cliente123") {
            const clienteUser = {
                nombre: "Cliente Prueba",
                usuario: "cliente",
                contrasena: "cliente123",
                rol: "Cliente"
            };
            setUser(clienteUser);
            localStorage.setItem("usuario_actual", JSON.stringify(clienteUser));
            navigate("/cliente");
            return;
        }

        // Login real con backend
        try {
            const requestData = {
                username: username,
                password: password,
                user_type: "",
                primary_key: ""
            };

            const response = await axios.post("https://localhost:7199/Login/user", requestData);

            if (response.data.status === "OK") {
                const data = response.data.message;

                const usuario = {
                    nombre: data.username,
                    usuario: data.username,
                    contrasena: data.password,
                    rol: data.user_type
                };

                setUser(usuario);
                localStorage.setItem("usuario_actual", JSON.stringify(usuario));

                setCuenta(data);
                localStorage.setItem("cuenta_actual", JSON.stringify(data));

                alert("Logeo exitoso");

                switch (data.user_type) {
                    case "Admin":
                        navigate("/admin");
                        break;
                    case "Client":
                        navigate("/cliente");
                        break;
                    case "Instructor":
                        navigate("/instructor");
                        break;
                    default:
                        alert("Rol desconocido");
                        break;
                }
            } else {
                alert("Login fallido: " + (response.data.message || "Respuesta no válida del servidor"));
            }
        } catch (error) {
            console.error(error);
            alert("Error al conectar con el servidor");
        }
    };

    return (
        <div className={styles.loginWrapper}>
            <div className={styles.wrapper}>
                <form onSubmit={handleLogin}>
                    <h1 className={styles.title}>GymTEC</h1>

                    <div className={styles.inputBox}>
                        <input type="text" placeholder="Usuario" value={username} onChange={(e) => setUsername(e.target.value)} required />
                        <FaRegUser className={styles.icon} />
                    </div>

                    <div className={styles.inputBox}>
                        <input
                            type={showPassword ? "text" : "password"}
                            placeholder="Contraseña"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            required
                        />
                        <MdLockOutline className={styles.icon} />
                    </div>

                    <div className={styles.checkboxContainer}>
                        <input
                            type="checkbox"
                            id="showPassword"
                            checked={showPassword}
                            onChange={(e) => setShowPassword(e.target.checked)}
                            className={styles.checkbox}
                        />
                        <label htmlFor="showPassword" className={styles.checkboxLabel}>Mostrar contraseña</label>
                    </div>

                    <button type="submit" className={styles.button}>Ingresar</button>
                </form>
            </div>
        </div>
    );
}

export default Login;
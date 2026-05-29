import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { FaRegUser } from "react-icons/fa";
import { MdLockOutline } from "react-icons/md";
import axios from "axios";
import styles from './Login.module.css';

function Login({ setUser }) {
    const [mode, setMode] = useState("login"); // "login" o "registro"
    const [form, setForm] = useState({
        email: "",
        password: "",
        alias: "",
        regPassword: "",
        country: "CR",
        preferred_language: "es",
        accept_disclosure: false
    });
    const [showPassword, setShowPassword] = useState(false);
    const navigate = useNavigate();

    const handleChange = e => {
        const { name, value } = e.target;
        setForm(prev => ({ ...prev, [name]: value }));
    };

    const handleLogin = async (e) => {
        e.preventDefault();

        // Logins locales de prueba
        if (form.username === "admin" && form.password === "admin123") {
            const adminUser = { nombre: "Admin", usuario: "admin", contrasena: "admin123", rol: "Admin" };
            setUser(adminUser);
            localStorage.setItem("usuario_actual", JSON.stringify(adminUser));
            navigate("/admin");
            return;
        }

        if (form.username === "instructor" && form.password === "instructor123") {
            const instructorUser = { nombre: "Instructor", usuario: "instructor", contrasena: "instructor123", rol: "Instructor" };
            setUser(instructorUser);
            localStorage.setItem("usuario_actual", JSON.stringify(instructorUser));
            navigate("/instructor");
            return;
        }

        if (form.username === "cliente" && form.password === "cliente123") {
            const clienteUser = { nombre: "Cliente", usuario: "cliente", contrasena: "cliente123", rol: "Cliente" };
            setUser(clienteUser);
            localStorage.setItem("usuario_actual", JSON.stringify(clienteUser));
            navigate("/cliente");
            return;
        }

        try {

            const requestData = {
                email: form.email,
                password: form.password
            };

            const response =
                await axios.post(
                    "https://localhost:7271/Login/login",
                    requestData
                );

            if (response.data.status) {

                const data = response.data.data;

                const usuario = {
                    user_id: data.user_id,
                    email: data.email,
                    alias: data.alias,
                    role: data.role
                };

                setUser(usuario);

                localStorage.setItem(
                    "usuario_actual",
                    JSON.stringify(usuario)
                );

                alert("Login exitoso");

                if (data.role === "ADMIN") {
                    navigate("/admin");
                }
                else {
                    navigate("/catalog");
                }
            }

        }
        catch (error) {

            console.error(error);

            alert("Correo o contraseña incorrectos");
        }
    };

    const handleRegister = async (e) => {

        e.preventDefault();

        try {

            const payload = {

                email: form.email,

                alias: form.alias,

                password: form.regPassword,

                country: form.country,

                preferred_language:
                form.preferred_language,

                accept_disclosure:
                form.accept_disclosure
            };

            const response =
                await axios.post(
                    "https://localhost:7271/Register/register",
                    payload
                );

            if (response.data.status) {

                alert(
                    "Registro exitoso. Ya puedes iniciar sesión."
                );

                setMode("login");
            }
        }
        catch (error) {

            console.error(error);

            alert(
                error.response?.data ||
                "Error al registrar usuario"
            );
        }
    };
    return (
        <div className={styles.loginWrapper}>
            <div className={styles.wrapper}>
                {mode === "login" ? (
                    <form onSubmit={handleLogin}>
                        <h1 className={styles.title}>PokeGrading</h1>
                        <div className={styles.inputBox}>
                            <input type="email" name="email" placeholder="Correo electrónico"
                                   value={form.email} onChange={handleChange} required />
                            <FaRegUser className={styles.icon} />
                        </div>
                        <div className={styles.inputBox}>
                            <input type={showPassword ? "text" : "password"} name="password" placeholder="Contraseña"
                                   value={form.password} onChange={handleChange} required />
                            <MdLockOutline className={styles.icon} />
                        </div>
                        <div className={styles.checkboxContainer}>
                            <input type="checkbox" id="showPassword" checked={showPassword}
                                   onChange={(e) => setShowPassword(e.target.checked)} className={styles.checkbox} />
                            <label htmlFor="showPassword" className={styles.checkboxLabel}>Mostrar contraseña</label>
                        </div>
                        <button type="submit" className={styles.button}>Ingresar</button>

                        {/* Selector de pestaña dentro del contenedor */}
                        <div className={styles.tabSelector}>
                            <span className={mode === "login" ? styles.active : ""}
                                  onClick={() => setMode("login")}>Ingreso</span>
                            <span className={mode === "registro" ? styles.active : ""}
                                  onClick={() => setMode("registro")}>Registro</span>
                        </div>
                    </form>
                ) : (
                    <form onSubmit={handleRegister}>
                        <h1 className={styles.title}>Registro Usuario</h1>

                        <div className={styles.formGrid}>

                            <div className={styles.column}>

                                <div className={styles.inputBox}>
                                    <input
                                        type="email"
                                        name="email"
                                        placeholder="Correo electrónico"
                                        value={form.email}
                                        onChange={handleChange}
                                        required
                                    />
                                </div>

                                <div className={styles.inputBox}>
                                    <input
                                        type="text"
                                        name="alias"
                                        placeholder="Alias"
                                        value={form.alias}
                                        onChange={handleChange}
                                        required
                                    />
                                </div>

                                <div className={styles.inputBox}>
                                    <input
                                        type="password"
                                        name="regPassword"
                                        placeholder="Contraseña"
                                        value={form.regPassword}
                                        onChange={handleChange}
                                        required
                                    />
                                </div>

                            </div>

                            <div className={styles.column}>

                                <div className={styles.inputBox}>
                                    <select
                                        name="country"
                                        value={form.country}
                                        onChange={handleChange}
                                    >
                                        <option value="CR">Costa Rica</option>
                                        <option value="PA">Panamá</option>
                                        <option value="MX">México</option>
                                        <option value="CO">Colombia</option>
                                        <option value="CL">Chile</option>
                                        <option value="AR">Argentina</option>
                                    </select>
                                </div>

                                <div className={styles.inputBox}>
                                    <select
                                        name="preferred_language"
                                        value={form.preferred_language}
                                        onChange={handleChange}
                                    >
                                        <option value="es">Español</option>
                                        <option value="en">English</option>
                                    </select>
                                </div>

                                <div className={styles.checkboxContainer}>

                                    <input
                                        type="checkbox"
                                        name="accept_disclosure"
                                        checked={form.accept_disclosure}
                                        onChange={(e) =>
                                            setForm(prev => ({
                                                ...prev,
                                                accept_disclosure: e.target.checked
                                            }))
                                        }
                                        className={styles.checkbox}
                                    />

                                    <label className={styles.checkboxLabel}>
                                        Acepto el disclosure de PokéGrading
                                    </label>

                                </div>

                            </div>

                        </div>

                        <div className={styles.formActions}>

                            <button
                                type="submit"
                                className={styles.button}
                            >
                                Registrarse
                            </button>

                            <div className={styles.tabSelector}>
                                <span
                                    className={mode === "login"
                                    ? styles.active
                                    : ""}
                             onClick={() => setMode("login")}
                            >
                                Ingreso
                            </span>

                                <span
                                    className={mode === "registro"
                                        ? styles.active
                                        : ""}
                                    onClick={() => setMode("registro")}
                                >
                                    Registro
                                </span>

                            </div>

                        </div>
                    </form>
                )}
            </div>
        </div>
    );
}

export default Login;
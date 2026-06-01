import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { FaRegUser } from "react-icons/fa";
import { MdLockOutline } from "react-icons/md";
import axios from "axios";
import styles from './Login.module.css';

function Login({ setUser }) {

    const [mode, setMode] = useState("login");

    const [form, setForm] = useState({
        email: "",
        password: "",
        alias: "",
        regPassword: "",
        country: "CR",
        preferred_language: "es",
        accept_disclosure: false
    });

    const [fieldErrors, setFieldErrors] = useState({});

    const [showPassword, setShowPassword] = useState(false);

    const navigate = useNavigate();

    const handleChange = (e) => {

        const { name, value, type, checked } = e.target;

        setForm(prev => ({
            ...prev,
            [name]: type === "checkbox"
                ? checked
                : value
        }));

        setFieldErrors(prev => ({
            ...prev,
            [name]: ""
        }));
    };

    const handleLogin = async (e) => {

        e.preventDefault();

        setFieldErrors({});

        try {

            const response =
                await axios.post(
                    "http://localhost:5164/Login/login",
                    {
                        email: form.email,
                        password: form.password
                    }
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

                if (data.role === "ADMIN") {
                    navigate("/admin");
                }
                else if (data.role === "SUBMITTER") {
                    navigate("/catalog");
                }
                else {
                    navigate("/");
                }
            }
        }
        catch (error) {

            if (error.response?.data?.field) {

                setFieldErrors({
                    [error.response.data.field]:
                    error.response.data.message
                });

            } else {

                alert("Error al iniciar sesión");
            }
        }
    };

    const handleRegister = async (e) => {

        e.preventDefault();

        setFieldErrors({});

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
                    "http://localhost:5164/Register/register",
                    payload
                );

            if (response.data.status) {

                alert(
                    "Registro exitoso. Ya puedes iniciar sesión."
                );

                setMode("login");

                setForm({
                    email: "",
                    password: "",
                    alias: "",
                    regPassword: "",
                    country: "CR",
                    preferred_language: "es",
                    accept_disclosure: false
                });
            }
        }
        catch (error) {

            if (error.response?.data?.field) {

                setFieldErrors({
                    [error.response.data.field]:
                    error.response.data.message
                });

            } else {

                alert("Error al registrar usuario");
            }
        }
    };

    return (
        <div className={styles.loginWrapper}>
            <div className={styles.wrapper}>

                {mode === "login" ? (

                    <form onSubmit={handleLogin}>

                        <h1 className={styles.title}>
                            PokeGrading
                        </h1>

                        <div className={styles.inputBox}>
                            <input
                                type="email"
                                name="email"
                                placeholder="Correo electrónico"
                                value={form.email}
                                onChange={handleChange}
                                required
                            />
                            <FaRegUser className={styles.icon} />
                        </div>

                        {fieldErrors.email &&
                            <span className={styles.errorText}>
                                {fieldErrors.email}
                            </span>
                        }

                        <div className={styles.inputBox}>
                            <input
                                type={showPassword ? "text" : "password"}
                                name="password"
                                placeholder="Contraseña"
                                value={form.password}
                                onChange={handleChange}
                                required
                            />
                            <MdLockOutline className={styles.icon} />
                        </div>

                        {fieldErrors.password &&
                            <span className={styles.errorText}>
                                {fieldErrors.password}
                            </span>
                        }

                        <div className={styles.checkboxContainer}>
                            <input
                                type="checkbox"
                                id="showPassword"
                                checked={showPassword}
                                onChange={(e) =>
                                    setShowPassword(e.target.checked)
                                }
                                className={styles.checkbox}
                            />
                            <label
                                htmlFor="showPassword"
                                className={styles.checkboxLabel}
                            >
                                Mostrar contraseña
                            </label>
                        </div>

                        <button
                            type="submit"
                            className={styles.button}
                        >
                            Ingresar
                        </button>

                        <div className={styles.tabSelector}>
                            <span
                                className={
                                    mode === "login"
                                        ? styles.active
                                        : ""
                                }
                                onClick={() => setMode("login")}
                            >
                                Ingreso
                            </span>

                            <span
                                className={
                                    mode === "registro"
                                        ? styles.active
                                        : ""
                                }
                                onClick={() => setMode("registro")}
                            >
                                Registro
                            </span>
                        </div>

                    </form>

                ) : (

                    <form onSubmit={handleRegister}>

                        <h1 className={styles.title}>
                            Registro Usuario
                        </h1>

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

                                {fieldErrors.email &&
                                    <span className={styles.errorText}>
                                        {fieldErrors.email}
                                    </span>
                                }

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

                                {fieldErrors.password &&
                                    <span className={styles.errorText}>
                                        {fieldErrors.password}
                                    </span>
                                }

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
                                        onChange={handleChange}
                                        className={styles.checkbox}
                                    />

                                    <label
                                        className={styles.checkboxLabel}
                                    >
                                        Acepto el disclosure de PokéGrading
                                    </label>

                                </div>

                                {fieldErrors.accept_disclosure &&
                                    <span className={styles.errorText}>
                                        {fieldErrors.accept_disclosure}
                                    </span>
                                }

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
                                    className={
                                        mode === "login"
                                            ? styles.active
                                            : ""
                                    }
                                    onClick={() => setMode("login")}
                                >
                                    Ingreso
                                </span>

                                <span
                                    className={
                                        mode === "registro"
                                            ? styles.active
                                            : ""
                                    }
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
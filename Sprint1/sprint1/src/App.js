import { BrowserRouter as Router, Routes, Route, Navigate } from "react-router-dom";
import { useState } from "react";
import Login from "./pgs/Login";
import AdminPg from "./pgs/AdminPg";
import ClientePg from "./pgs/ClientePg";


import 'bootstrap/dist/css/bootstrap.min.css';

function App() {
  const [user, setUser] = useState(null);

  return (
      <Router>
        <Routes>
          <Route path="/login" element={<Login setUser={setUser} />} />

          <Route path="/admin" element={user?.rol === "Admin" ? <AdminPg /> : <Navigate to="/login" />} />

          <Route path="/cliente" element={user?.rol === "Cliente" ? <ClientePg /> : <Navigate to="/login" />} />

          <Route path="*" element={<Navigate to="/login" />} />
        </Routes>
      </Router>
  );
}
export default App;
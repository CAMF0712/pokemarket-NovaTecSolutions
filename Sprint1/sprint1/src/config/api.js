const DEFAULT_API_BASE_URL =
    `https://${window.location.hostname}:7271`;

export const API_BASE_URL =
    process.env.REACT_APP_API_BASE_URL ||
    DEFAULT_API_BASE_URL;

export const buildApiUrl = (path) =>
    `${API_BASE_URL}${path}`;

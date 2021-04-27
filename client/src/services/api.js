import axios from "axios";

const api = axios.create({
    baseURL: process.env.NODE_ENV === 'development' ? process.env.REACT_APP_API_URL_LOCAL : process.env.REACT_APP_API_URL_HEROKU,
});

export default api;
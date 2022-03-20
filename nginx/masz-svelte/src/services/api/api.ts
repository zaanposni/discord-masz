import axios from "axios";
import { API_URL, ENABLE_CORS } from "../../config";

const axiosAPI = axios.create({
    baseURL: API_URL,
});
const axiosAssetAPI = axios.create();

const getAsset = (url) => {
    return axiosAssetAPI({
        url,
        method: "GET",
    })
        .then((res) => {
            return Promise.resolve(res.data);
        })
        .catch((err) => {
            console.error(err);
            return Promise.reject(err);
        });
};

const apirequest = (method, url, data) => {
    return axiosAPI({
        method,
        url,
        data: data,
        withCredentials: ENABLE_CORS,
    })
        .then((res) => {
            return Promise.resolve(res.data);
        })
        .catch((err) => {
            console.error(err);
            return Promise.reject(err);
        });
};

const get = (url) => apirequest("get", url, undefined);
const deleteData = (url, data) => apirequest("delete", url, data);
const post = (url, data) => apirequest("post", url, data);
const put = (url, data) => apirequest("put", url, data);
const patch = (url, data) => apirequest("patch", url, data);

const API = {
    getAsset,
    get,
    deleteData,
    post,
    put,
    patch,
};
export default API;

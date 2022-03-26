import axios from "axios";
import { API_CACHE_CLEAR_LIFETIME, API_CACHE_ENABLE, API_CACHE_LIFETIME, API_URL, APP_BASE_URL, ENABLE_CORS } from "../../config";
import { CacheMode } from "./CacheMode";

const axiosAPI = axios.create({
    baseURL: API_URL,
});
const axiosStaticAPI = axios.create({
    baseURL: APP_BASE_URL
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

const getStatic = (url) => {
    return axiosStaticAPI({
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

function saveResponseInCache(method, url, response) {
    localStorage.setItem(`apicache:${method}:${url}`, JSON.stringify({
        response,
        timestamp: Date.now(),
    }));
}

function getResponseFromCache(method, url) {
    const item = localStorage.getItem(`apicache:${method}:${url}`);
    if (item) {
        const json = JSON.parse(item);
        if (Date.now() - json.timestamp < API_CACHE_LIFETIME) {
            return json.response;
        } else {
            localStorage.removeItem(`apicache:${method}:${url}`);
        }
    }
    return null;
}

// clear all entries older than API_CACHE_CLEAR_LIFETIME from the api cache
function clearCache(): number {
    let count = 0;
    for (var i = 0; i < localStorage.length; i++) {
        const key = localStorage.key(i);
        if (key.startsWith("apicache:")) {
            // check for timestamp older than API_CACHE_CLEAR_LIFETIME
            const item = localStorage.getItem(key);
            if (item) {
                const json = JSON.parse(item);
                if (Date.now() - json.timestamp > API_CACHE_CLEAR_LIFETIME) {
                    count++;
                    localStorage.removeItem(key);
                }
            } else {
                count++;
                localStorage.removeItem(key);
            }
        }
    }
    return count;
}

const get = (url, cacheMode: CacheMode = CacheMode.API_ONLY, saveInCache: boolean = false) => {
    if ((cacheMode === CacheMode.CACHE_ONLY || cacheMode === CacheMode.PREFER_CACHE) && API_CACHE_ENABLE) {
        const response = getResponseFromCache("get", url);
        if (response) {
            return Promise.resolve(response);
        }
        if (cacheMode === CacheMode.CACHE_ONLY) {
            return Promise.reject("No data in cache");
        }
    }
    const apiPromise = apirequest("get", url, undefined);
    return apiPromise.then((response) => {
        if (saveInCache && API_CACHE_ENABLE) {
            saveResponseInCache("get", url, response);
        }
        return response;
    });
}
const deleteData = (url, data) => apirequest("delete", url, data);
const post = (url, data, cacheMode: CacheMode = CacheMode.API_ONLY, saveInCache: boolean = false) => {
    if ((cacheMode === CacheMode.CACHE_ONLY || cacheMode === CacheMode.PREFER_CACHE) && API_CACHE_ENABLE) {
        const response = getResponseFromCache("post", url);
        if (response) {
            return Promise.resolve(response);
        }
        if (cacheMode === CacheMode.CACHE_ONLY) {
            return Promise.reject("No data in cache");
        }
    }
    const apiPromise = apirequest("post", url, data);
    return apiPromise.then((response) => {
        if (saveInCache && API_CACHE_ENABLE) {
            saveResponseInCache("post", url, response);
        }
        return response;
    });
}
const put = (url, data) => apirequest("put", url, data);
const patch = (url, data) => apirequest("patch", url, data);

const API = {
    clearCache,
    getAsset,
    getStatic,
    get,
    deleteData,
    post,
    put,
    patch,
};
export default API;

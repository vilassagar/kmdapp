import axios from "axios";

const axiosInstance = () => {
  const instance = axios.create({
    baseURL: `${import.meta.env.VITE_API_BASE_URL}/api`,
    headers: {
      // Default headers - Content-Type will be overridden when needed
      'Accept': 'application/json'
    }
  });

  // Add no-cache headers and cache-busting interceptors
  instance.interceptors.request.use(async function (config) {
    // Check if request contains FormData
    const isFormData = config.data instanceof FormData;
    
    // Set cache-control and basic headers, but let Axios set the Content-Type for FormData
    config.headers = {
      ...config.headers,
      'Accept': 'application/json',
      'Cache-Control': 'no-store, no-cache, must-revalidate, max-age=0',
      'Pragma': 'no-cache',
      'Expires': '0',
    };
    
    // Only set Content-Type for JSON, not for FormData (browser will set it correctly with boundary)
    if (!isFormData) {
      config.headers['Content-Type'] = 'application/json';
    }

    // Add cache-busting timestamp
    config.params = {
      ...config.params,
      '_': Date.now(), // Unique timestamp to prevent caching
    };

    // Existing token and user state logic
    let userState = getUserState();

    if(userState?.state?.user!==null) {
      // Check if token is expired and needs refresh
      if (userState && isTokenExpired(userState.state.user.expiresAt)) {
        try {
          // Refresh token
          const newUserData = await refreshToken(userState.state.user.refreshToken);
          
          // Update user state in localStorage
          userState.state.user = {
            ...userState.state.user,
            token: newUserData.token,
            refreshToken: newUserData.refreshToken,
            expiresAt: newUserData.expiresAt,
            createdAt: new Date().toISOString()
          };
          
          saveUserState(userState);
        } catch (error) {
          // If refresh fails, user will be redirected to login
          return Promise.reject(error);
        }
      }
    }

    // Re-fetch user state after potential refresh
    userState = getUserState();
    const token = userState?.state?.user?.token || "";

    // Add token to headers if exists
    config.headers = {
      ...config.headers,
      ...(token?.length ? { Authorization: `Bearer ${token}` } : {}),
    };

    return config;
  });

  // Response interceptor
  instance.interceptors.response.use(
    function (response) {
      // Prevent caching of successful responses
      response.headers['Cache-Control'] = 'no-store, no-cache, must-revalidate, max-age=0';
      return response;
    },
    function (error) {
      const { response, config } = error;
      
      // Bypass for specific routes
      const bypassRoutes = [
        "/authenticate/login", 
        "/authenticate/verifyotp", 
        "/authenticate/refresh-token"
      ];
      if (bypassRoutes.some(route => config?.url?.endsWith(route))) {
        return Promise.reject(error);
      }
      
      // Handle 401 (Unauthorized) errors
      if (response && response.status === 401) {
        // Remove user from local storage
        localStorage.removeItem("user");
        // Redirect to the login page
        window.location.href = "/login";
      }
      
      return Promise.reject(error);
    }
  );

  // Add utility methods for common content types
  // Regular JSON POST request
  instance.postJSON = (url, data, config = {}) => {
    return instance.post(url, data, {
      ...config,
      headers: {
        ...config.headers,
        'Content-Type': 'application/json'
      }
    });
  };

  // FormData POST request
  instance.postFormData = (url, formData, config = {}) => {
    // Ensure formData is actually a FormData instance
    if (!(formData instanceof FormData)) {
      const newFormData = new FormData();
      // If it's an object, convert to FormData
      if (formData && typeof formData === 'object') {
        Object.keys(formData).forEach(key => {
          newFormData.append(key, formData[key]);
        });
      }
      formData = newFormData;
    }
    
    return instance.post(url, formData, {
      ...config,
      headers: {
        ...config.headers,
        // Don't set Content-Type here - browser will do this automatically
      }
    });
  };

  // Mixed content type - JSON data with file uploads
  instance.postMixed = (url, jsonData, files, config = {}) => {
    const formData = new FormData();
    
    // Add JSON data as a string field
    formData.append('jsonData', JSON.stringify(jsonData));
    
    // Add files (can be single file or array of files)
    if (Array.isArray(files)) {
      files.forEach((file, index) => {
        formData.append(`file${index}`, file);
      });
    } else if (files) {
      formData.append('file', files);
    }
    
    return instance.postFormData(url, formData, config);
  };

  return instance;
};

// Existing utility functions
const getUserState = () => {
  const userString = localStorage.getItem("user");
  return userString ? JSON.parse(userString) : null;
};

const saveUserState = (userState) => {
  localStorage.setItem("user", JSON.stringify(userState));
};

const isTokenExpired = (expiresAt) => {
  return new Date(expiresAt) <= new Date();
};

const refreshToken = async (refreshToken) => {
  try {
    const response = await axios.post(
      `${import.meta.env.VITE_API_BASE_URL}/api/authenticate/refresh-token`,
      { refreshToken },
      {
        headers: {
          'Content-Type': 'application/json',
          'Accept': 'application/json'
        }
      }
    );
    return response.data;
  } catch (error) {
    // If refresh token fails, logout user
    localStorage.removeItem("user");
    window.location.href = "/login";
    throw error;
  }
};

const httpClient = axiosInstance();

export default httpClient;
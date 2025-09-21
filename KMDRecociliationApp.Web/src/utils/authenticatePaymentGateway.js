import axios from "axios";


const loginUrl = 'https://preprod-oneapi.godigit.com/OneAPI/v1/auth'; // Replace with your API endpoint

const requestData = {
    'data-raw': {
         username : "AfSgwJNG1l4nB3U+YBCloA==",
    password: "v4lHHEwiPGXFZl8N3ZmFIg=="
    }
};

export const authenticateFunction = () => {
    axios.post(loginUrl,requestData['data-raw'], {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Cookie': 'TS0198aefb=0138ecebf96f9531dbd1898da30784872bcbd4f716d17f5423e48c53afadbbb5cfac82ca74daafc65c6421f6c98b5da9a11308e8cc; TS017fdda2=0138ecebf9c712e9cc47f6c40dc63ff7f013cbbdf802e1651129751c8fef1eeb0ffd0be127c7edc77edfc0fdd4e7f15cedd85f8db8' // Include if your API needs cookies
        },
        
    })
    .then(response => response.json())
    .then(data => {
        const token = data.access_token;
        console.log("Token received: ", token);
    
      
        makeAuthenticatedRequest(token);
    })
    .catch(error => {
        console.error('Error during login:', error);
    });
}




function makeAuthenticatedRequest(token) {
    const apiUrl = 'https://uat-oneapi.godigit.com/OneAPI/v1/executor'; 

    axios.get(apiUrl, {
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}` 
        }
    })
    .then(response => {
        console.log("Response from authenticated API: ", response.data);
    })
    .catch(error => {
        console.error('Error during authenticated request:', error);
    });
}

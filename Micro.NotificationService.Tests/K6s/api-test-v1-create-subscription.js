import http from 'k6/http';
import { check, sleep } from 'k6';

//const baseUrl = 'http://localhost:8085'; //Docker
const baseUrl = 'http://localhost:5112'; //Debug

export let options = {
    stages: [
        { duration: '10s', target: 20 },
        { duration: '50s', target: 20 }, 
    ],
};

// Function to generate a random GUID
function generateGUID() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0, v = c === 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}

export default function () {

    const apiUrl = `${baseUrl}/api/v1/subscriptions`;

    const request = {
        userId: generateGUID(),
        subscriberName: "Test",
        emailAddress: "test@test.com",
        notificationType: "Type",
        notificationChannel: 0,
        isSubscribed : true
    };

    const response = http.post(apiUrl, JSON.stringify(request), {
        headers: { 'Content-Type': 'application/json' }
    });

    check(response, {
        'response code was 200': (res) => res.status == 200
    });
}
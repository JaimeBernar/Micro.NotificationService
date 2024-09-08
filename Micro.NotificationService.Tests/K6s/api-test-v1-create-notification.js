import http from 'k6/http';
import { check, sleep } from 'k6';

const baseUrl = 'http://localhost:8085';
//const baseUrl = 'http://localhost:5112'; //Debug

export let options = {
    stages: [
        { duration: '10s', target: 20 },  // Ramp-up to 20 users
        { duration: '50s', target: 20 },  // Ramp-up to 20 users
    ],
};

export function setup() {

    const apiUrl = `${baseUrl}/api/v1/subscriptions`;

    const request = {
        userId: "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        subscriberName: "TestSubscriber",
        emailAddress: "test@email.com",
        notificationType: "Type",
        channel: 0,
        isSubscribed: true
    };

    const response = http.post(apiUrl, JSON.stringify(request), {
        headers: { 'Content-Type': 'application/json' }
    });

    check(response, {
        'POST request succeeded': (res) => res.status === 201 || res.status === 200,
    });
}

export default function () {

    const apiUrl = `${baseUrl}/api/v1/notifications`;

    const request = {
        notificationType: "Type",
        channel: 0,
        header: "Header",
        body: "Body"
    };

    const response = http.post(apiUrl, JSON.stringify(request), {
        headers: { 'Content-Type': 'application/json' }
    });

    check(response, {
        'response code was 200': (res) => res.status == 200
    });
}

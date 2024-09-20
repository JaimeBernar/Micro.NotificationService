import http from 'k6/http';
import { check, sleep } from 'k6';

//const baseUrl = 'http://localhost:8085';
const baseUrl = 'http://localhost:5112'; //Debug

export let options = {
    stages: [
        { duration: '10s', target: 20 },
        { duration: '50s', target: 20 }, 
    ],
};

export function setup() {

    const apiUrl = `${baseUrl}/api/v1/subscriptions`;

    const emailSubscription = {
        userId: "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        subscriberName: "TestSubscriber",
        emailAddress: "test@email.com",
        notificationType: "Type",
        channel: 0,
        isSubscribed: true
    };

    const webSubscription = {
        userId: "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        subscriberName: "TestSubscriber",
        emailAddress: "test@email.com",
        notificationType: "Type",
        channel: 1,
        isSubscribed: true
    };

    const emailResponse = http.post(apiUrl, JSON.stringify(emailSubscription), {
        headers: { 'Content-Type': 'application/json' }
    });

    const webResponse = http.post(apiUrl, JSON.stringify(webSubscription), {
        headers: { 'Content-Type': 'application/json' }
    });

    check(emailResponse, {
        'POST request for Email Subscription succeeded': (res) => res.status === 201 || res.status === 200,
    });

    check(webResponse, {
        'POST request for Web Subscription succeeded': (res) => res.status === 201 || res.status === 200,
    });
}

export default function () {

    const apiUrl = `${baseUrl}/api/v1/notifications`;

    const request = {
        notificationType: "Type",
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
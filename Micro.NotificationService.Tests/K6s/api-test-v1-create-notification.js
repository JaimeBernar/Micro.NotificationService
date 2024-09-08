import http from 'k6/http';
import { check, sleep } from 'k6';

export let options = {
    stages: [
        { duration: '10s', target: 20 },  // Ramp-up to 20 users
        { duration: '50s', target: 20 },  // Ramp-up to 20 users
    ],
};

export default function () {
    const apiUrl = 'http://localhost:8085/api/v1/notifications';

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
        'response code was 500': (res) => res.status == 500
    });
}

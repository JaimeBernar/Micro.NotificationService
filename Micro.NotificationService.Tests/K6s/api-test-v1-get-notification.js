import http from 'k6/http';
import { check, sleep } from 'k6';

export let options = {
    stages: [
        { duration: '10s', target: 20 },  // Ramp-up to 20 users
        { duration: '50s', target: 20 },  // Ramp-up to 20 users
    ],
};

export default function () {
    const apiUrl = 'http://localhost:8085/api/v1/notifications/3fa85f64-5717-4562-b3fc-2c963f66afa6';

    const response = http.get(apiUrl, {
        headers: { 'Content-Type': 'application/json' }
    });

    check(response, {
        'response code was 200': (res) => res.status == 200
    });
}

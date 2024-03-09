import http from 'k6/http'

export const options = {
    vus: 200,
    duration: '10s',
    thresholds: {
        http_req_duration: ['p(95)<600'],
    },
};

export default function () {
    const baseAddress = "https://localhost:5006/article/"

    http.get(baseAddress + 'OAuth2.0-in-simple-words')
    http.get(baseAddress + 'Synchronization-by-lock-and-Monitor-statements-part-2')
    http.get(baseAddress + 'why-should-you-use-spant-in-dotnet')
}
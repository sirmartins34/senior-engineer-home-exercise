function ci-up {
    Write-Host "Starting CI stack with TeamCity and local registry..."
    docker compose -f compose.ci.yml up --build -d

    $url = "http://localhost:8111"
    $maxRetries = 30
    $retryDelay = 5

    for ($i = 0; $i -lt $maxRetries; $i++) {
        try {
            $response = Invoke-WebRequest -Uri $url -UseBasicParsing -TimeoutSec 5
            if ($response.StatusCode -eq 200) {
                Write-Host "TeamCity is ready at $url"
                return
            }
        } catch {
            Write-Host "Waiting for TeamCity server... ($i/$maxRetries)"
        }
        Start-Sleep -Seconds $retryDelay
    }

    Write-Error "TeamCity server did not respond after waiting $($maxRetries * $delay) seconds."
}
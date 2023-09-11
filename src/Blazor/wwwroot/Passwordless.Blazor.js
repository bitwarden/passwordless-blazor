import "./Passwordless.min.js";

export function initializePasswordlessClient(apiKey, apiUrl) {
  return new Passwordless.Client({apiKey, apiUrl});
}

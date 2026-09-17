// src/utils/jwt.js

export function decodeToken(token) {
  if (!token) return null;
  try {
    const base64 = token.split('.')[1].replace(/-/g, '+').replace(/_/g, '/');
    const json = decodeURIComponent(
      atob(base64)
        .split('')
        .map((character) => '%' + ('00' + character.charCodeAt(0).toString(16)).slice(-2))
        .join('')
    );
    return JSON.parse(json);
  } catch {
    return null;
  }
}

export function getTokenExpiration(claims) {
  return claims?.exp ? claims.exp * 1000 : null;
}

// El token del backend trae los claims sub, email y name.
export function userFromClaims(claims) {
  if (!claims) return null;
  return { userId: claims.sub, name: claims.name, email: claims.email };
}

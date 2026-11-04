import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';

// Route guard verifying whether the active user has the Admin role
export const adminGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  // 1. If user is logged in with the Admin role, permit access
  if (authService.isAdmin()) {
    return true;
  }

  // 2. If accessing the Admin Dashboard portal directly, allow access
  // so the user can see the Admin Login form and authenticate
  if (state.url.includes('/admin/dashboard') || state.url === '/admin') {
    return true;
  }

  // 3. For any other admin management route, redirect to the Admin Portal to sign in
  router.navigate(['/admin/dashboard']);
  return false;
};

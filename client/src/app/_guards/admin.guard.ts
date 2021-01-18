import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { AccountService } from '../_services/account.service';

@Injectable({
  providedIn: 'root'
})
export class AdminGuard implements CanActivate {

  // constructor so we can activate acc service
  constructor(private accountServicve: AccountService, private toastr: ToastrService) {}

  canActivate(): Observable<boolean> {
    return this.accountServicve.currentUser$.pipe(
      // check which roles, use map so we can use a callback function on the user
      map(user => {
        if (user.roles.includes('Admin') || user.roles.includes('Moderator')) {
          return true;
        }
        this.toastr.error('You cannot enter this area');
      })
    )
  }
  
}

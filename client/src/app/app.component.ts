import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { User } from './_models/user';
import { AccountService } from './_services/account.service';
import { PresenceService } from './_services/presence.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'The Dating App';
  users: any;

  constructor(private accountService: AccountService, private presence: PresenceService) {}

  ngOnInit() {
    // Get User object From Local Storage
    this.setCurrentUser();
  }

  setCurrentUser() {
    // Get User from local storage
    const user: User = JSON.parse(localStorage.getItem('user'));
    // check that we have a user
    if (user) {
      // Set user in acc server
      this.accountService.setCurrentuser(user);
      this.presence.createHubConnection(user); // get access to users jwt token
    }

  }

}

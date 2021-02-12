import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { BehaviorSubject } from 'rxjs';
import { CombineLatestOperator } from 'rxjs/internal/observable/combineLatest';
import { take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';
import { UserParams } from '../_models/userParams';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {
  hubUrl = environment.hubUrl;
  private hubConnection: HubConnection;
  // create observable - behavior requires initial value
  private onlineUsersSource = new BehaviorSubject<string[]>([]);
  onlineUsers$ = this.onlineUsersSource.asObservable();

  constructor(private toastr: ToastrService, private router: Router) { }

  // connect to app and are authorized connect to presence hub
  createHubConnection(user: User){
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'presence', {
        accessTokenFactory: () => user.token
      })
      .withAutomaticReconnect() // if network problem recconect
      .build()

      // start hub connection
      this.hubConnection
        .start()
        .catch(error => console.log(error));

      // listen for server events
      this.hubConnection.on('UserIsOnline', username => {
        // can add notification here for users to be notified when others online
        this.onlineUsers$.pipe(take(1)).subscribe(usernames => {
          this.onlineUsersSource.next([...usernames, username])
        })
      })

      this.hubConnection.on('UserIsOffline', username => {
        this.onlineUsers$.pipe(take(1)).subscribe(usernames => {
          this.onlineUsersSource.next([...usernames.filter(x => x !== username)])
        })
      })

      this.hubConnection.on('GetOnlineUsers', (usernames: string[]) => {
        this.onlineUsersSource.next(usernames);
      })      
      
      this.hubConnection.on('NewMessageRecieved', ({username, knownAs}) => {
        this.toastr.info('New message from ' + knownAs)
          .onTap
          .pipe(take(1))
          .subscribe(() => this.router.navigateByUrl('/members/' + username + '?tab=3'));
      })
  }

  // stop hub connection
  stopHubConnection() {
    this.hubConnection.stop().catch(error => console.log(error));
  }
}

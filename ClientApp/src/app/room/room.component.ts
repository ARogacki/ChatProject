import { ChangeDetectorRef, Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Message } from '../Message';
import { SignalrService } from '../signalr.service';

@Component({
  selector: 'app-room',
  templateUrl: './room.component.html',
  styleUrls: ['./room.component.css']
})
export class RoomComponent implements OnInit {
  title = 'chat-ui';
  text: string = "";
  public messages: Message[] = [];
  username: string;
  room: string;
  connected: boolean;

  constructor(private changeDetection: ChangeDetectorRef, private router: Router, private activatedRoute: ActivatedRoute, public signalRService: SignalrService) {
    if (this.router.getCurrentNavigation().extras.state)
      this.username = this.router.getCurrentNavigation().extras.state.username;
    this.room = this.activatedRoute.snapshot.paramMap.get('id');
  }

  ngOnInit() {
    if (this.username == null || this.room == null) {
      this.router.navigateByUrl('/');
    }
    else if (this.room.length < 1) {
      this.router.navigateByUrl('/');
    }
    else if (this.username.length < 1) {
      this.router.navigateByUrl('/');
    }
    else {
      this.connected = false;
      this.signalRService.connect();
      this.subscribeToEvents();
    }
  }

  private subscribeToEvents(): void {
    console.log(this.connected);
    if (!this.connected) {
      this.signalRService.messageReceived.subscribe(_ => {
        this.connected = true;
        this.signalRService.JoinRoom(this.room, this.username);
      });
    }
    this.signalRService.messageReceived.subscribe(_ => {
      this.changeDetection.detectChanges();
    });
  }

  updateState(): void {
    if (this.text.length < 1) {
      this.signalRService.changeUserState("chillin'").subscribe({
        error: (err) => console.error(err)
      })
    }
    else {
      this.signalRService.changeUserState("writing..").subscribe({
        error: (err) => console.error(err)
      })
    }
  }

  sendMessage(): void {
    this.signalRService.changeUserState("chillin'").subscribe({
      error: (err) => console.error(err)
    })
    this.signalRService.sendMessageToGroup(this.username, this.text).subscribe({
      next: _ => this.text = '',
      error: (err) => console.error(err)
    });
  }

}

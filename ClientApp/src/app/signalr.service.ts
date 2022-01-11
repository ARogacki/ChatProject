import { HttpClient } from '@angular/common/http';
import { ApplicationRef, EventEmitter, Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr'
import { from } from 'rxjs';
import { tap } from 'rxjs/operators';
import { Message } from './Message';
import { MessagePackHubProtocol } from '@microsoft/signalr-protocol-msgpack'
import { User } from './User';
@Injectable({
  providedIn: 'root'
})
export class SignalrService {

  private hubConnection: HubConnection
  public messages: Message[] = [];
  public users: User[] = [];
  private connectionUrl = 'https://localhost:44373/signalr';
  //private connectionUrl = 'https://localhost:' + window.location.port + '/signalr';
  //private apiUrl = 'https://localhost:' + window.location.port + '/api/chat';
  messageReceived = new EventEmitter<Message[]>();

  constructor(private ref: ApplicationRef, private http: HttpClient) {
  }

  public connect = () => {
    this.startConnection();
    this.addListeners();
  }

  public sendMessageToHub(username: string, message: string) {
    var promise = this.hubConnection.invoke("BroadcastAsync", this.buildChatMessage(username, message))
      .then(() => { console.log('message sent successfully to hub'); })
      .catch((err) => console.log('error while sending a message to hub: ' + err));

    return from(promise);
  }

  public changeUserState(state: string) {
    var promise = this.hubConnection.invoke("ChangeState", state)
      .then(() => { console.log('state successfully changed'); })
      .catch((err) => console.log('error while changing state: ' + err));

    return from(promise);
  }

  public sendMessageToGroup(username: string, message: string) {
    var promise = this.hubConnection.invoke("SendMessageToGroup", this.buildChatMessage(username, message))
      .then(() => { console.log('message sent successfully to group'); })
      .catch((err) => console.log('error while sending a message to group: ' + err));

    return from(promise);
  }

  public JoinRoom(room: string, username: string) {
    var promise = this.hubConnection.invoke("JoinRoom", room, username, this.buildChatMessage(username, username + " joined the room."))
      .then(() => { console.log('joined room'); })
      .catch((err) => console.log('error while attempting to join: ' + err));

    return from(promise);
  }

  private registerOnServerEvents(): void {
    this.hubConnection.on('successfullyConnected', _ => {
      console.log("Connected");
      this.messageReceived.emit(this.messages);
    });
  }

  private getConnection(): HubConnection {
    return new HubConnectionBuilder()
      .withUrl(this.connectionUrl)
      .withHubProtocol(new MessagePackHubProtocol())
      //  .configureLogging(LogLevel.Trace)
      .build();
  }

  private buildChatMessage(username: string, message: string): Message {
    return {
      ConnectionId: this.hubConnection.connectionId,
      Author: username,
      Text: message,
      DateTime: new Date()
    };
  }

  private startConnection(): any {
    this.hubConnection = this.getConnection();

    this.hubConnection.start()
      .then(() => console.log('connection started'))
      .catch((err) => console.log('error while establishing signalr connection: ' + err))

    this.registerOnServerEvents();
  }

  private addListeners() {
    this.hubConnection.on("messageReceivedFromHub", (data: Message) => {
      console.log("message received from Hub")
      this.messages.push(data);
    })
    this.hubConnection.on("newUserConnected", value => {
      console.log("new user connected")
      console.log(value);
      this.ref.tick();
    })
    this.hubConnection.on("SendAsync", (data: Message) => {
      this.messages.push(data);
      this.ref.tick();
    })
    this.hubConnection.on("ReceiveHistory", (data: Message[]) => {
      this.messages = data;
      this.ref.tick();
    })
    this.hubConnection.on("ReceiveUsers", (data: User[]) => {
      this.users = data;
      this.ref.tick();
    })
    this.hubConnection.on("UserDisconnected", value => {
      console.log("user disconnected");
      console.log(value);
      this.ref.tick();
    })
    this.hubConnection.on("SendError", value => {
      console.log("Exception caught:");
      console.log(value);
      this.ref.tick();
    })

  }

}

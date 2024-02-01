import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import {FormControl, ReactiveFormsModule} from "@angular/forms";
import {BaseDto, ServerEchosClientDto} from "../BaseDto";

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, ReactiveFormsModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'frontend';

  messages: string[] = [];

  ws: WebSocket = new WebSocket("ws://localhost:8181")
  messageContent =
    new FormControl('');

  constructor() {
    this.ws.onmessage = message => {
      const messageFromServer = JSON.parse(message.data) as BaseDto<any>;
      // @ts-ignore
      this[messageFromServer.eventType].call(this, messageFromServer);
    }
  }

  ServerEchosClient(dto: ServerEchosClientDto) {
    this.messages.push(dto.echoValue!);
  }


  sendMessage() {
    //Post-recording note: If you just make a class and instantiate it, the eventType is automatically assigned
    var object = {
      eventType: "ClientWantsToEchoServer",
      messageContent: this.messageContent.value!
    }
    this.ws.send(JSON.stringify(object));
  }
}

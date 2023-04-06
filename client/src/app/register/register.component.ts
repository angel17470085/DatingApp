import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Input() usersFromHomeComponent : any; // from parent to child communication
  @Output () cancelRegister = new EventEmitter(); // from child to parent communication
  model : any = {};
  constructor() { }

  ngOnInit(): void {
  }


  register (){
    console.log(this.model);
  }

  cancel(){
    console.log("cancelled");
    this.cancelRegister.emit(false);
  }
}

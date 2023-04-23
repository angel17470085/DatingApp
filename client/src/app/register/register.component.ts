import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';
import { FormControl, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Output () cancelRegister = new EventEmitter(); // from child to parent communication
  model : any = {};
  public registerForm : FormGroup = new FormGroup({});

  constructor(private accountService : AccountService,
    private toastr : ToastrService) { }

  ngOnInit(): void {
    this.initializeForm();
  }


  initializeForm() {
    this.registerForm = new FormGroup({
      username : new FormControl("hello",Validators.required),
      password: new FormControl("", [Validators.required, Validators.minLength(4), Validators.maxLength(8)]),
      confirmPassword: new FormControl("", Validators.required),
    })
  }
  register() {

    console.log(this.registerForm?.value)
    // this.accountService.register(this.model).subscribe({
    //   next: () => {
    //     this.cancel();
    //   },
    //   error: error => {
    //     this.toastr.error(error.error),
    //     console.log(error)
    //   }
    // })
  }

  cancel(){
    this.cancelRegister.emit(false);
  }
}

import { Component, OnInit, Output, EventEmitter } from "@angular/core";
import { AuthService } from "../_services/auth.service";
declare let alertify: any;

@Component({
  selector: "app-register",
  templateUrl: "./register.component.html",
  styleUrls: ["./register.component.css"]
})
export class RegisterComponent implements OnInit {
  @Output() closeRegisterModal = new EventEmitter();
  model: any = {};
  constructor(private authService: AuthService) {}

  ngOnInit() {}

  register() {
    this.authService.register(this.model).subscribe( () => {
      alertify.success('Registration Successful');
    }, error => {
      alertify.error(error.message);
    });
  }

  cancel() {
    this.closeRegisterModal.emit(false);
  }
}

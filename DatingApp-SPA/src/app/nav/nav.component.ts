import { Component, OnInit } from "@angular/core";
import { AuthService } from "../_services/auth.service";
import { BsDropdownModule } from 'ngx-bootstrap';
import { Router } from '@angular/router';
declare let alertify;

@Component({
  selector: "app-nav",
  templateUrl: "./nav.component.html",
  styleUrls: ["./nav.component.css"]
})
export class NavComponent implements OnInit {
  model: any = {};
  constructor(public authService: AuthService, private router: Router) {}

  ngOnInit() {}
  login() {
    console.log(this.model);
    this.authService
      .login(this.model)
      .subscribe(
        next => alertify.success("Sucessfull Login"),
        error => alertify.error(error), //error will be the message
        () => this.router.navigate(['/members'])
      );
  }

  loggedIn() {
    return this.authService.loggedIn();
  }

  logout(){
    localStorage.removeItem('token');
    alertify.message('logged out');
    this.router.navigate(['/home']);
  }
}

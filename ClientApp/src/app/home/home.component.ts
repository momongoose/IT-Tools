import { Component } from '@angular/core';
import { AppComponent } from '../app.component';
import { LoginService } from '../services/login.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent {
  constructor(private app : AppComponent, private log : LoginService) {}
  user = ""
  pass = ""
  login = this.app.login
  data : any = {}

Submit(){
  this.data["Id"] = 0
  this.data["Username"] = this.user
  this.data["Password"] = this.pass
  this.log.login(this.data).subscribe((dat)=>{
    return dat
  })
}

}

import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Output() cancelRegister = new EventEmitter();
  model:any={};
  constructor(private accountService:AccountService,private router:Router, private toastr:ToastrService) { }

  ngOnInit(): void {
  }
  register(){
    this.accountService.register(this.model).subscribe(response =>{
      console.log(response);
      this.router.navigateByUrl("/members");
      this.cancel();
    }, error => {
      console.log(error);
      this.toastr.error(error.error);
    });
    
  }
  cancel(){
    this.cancelRegister.emit(false);
  }

}

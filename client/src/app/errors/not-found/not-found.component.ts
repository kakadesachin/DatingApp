import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';

@Component({
  selector: 'app-not-found',
  templateUrl: './not-found.component.html',
  styleUrls: ['./not-found.component.css']
})
export class NotFoundComponent implements OnInit {

  constructor(private accountService:AccountService, private router: Router) { }

  ngOnInit(): void {
  }
  redirectToHome(){
    var user:User = null!;
    this.accountService.currentUser$.subscribe(response => {
      user=response
    },error => {
      user = null!;
    }
    );

    if (user) {
      this.router.navigateByUrl("/members");
    }
    else{
      this.router.navigateByUrl("/");
    }
  }

}

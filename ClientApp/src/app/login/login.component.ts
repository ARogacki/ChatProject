import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  private loginForm: FormGroup;
  append: boolean = false;
  suggestions = []
  constructor(private formBuilder: FormBuilder, private route: ActivatedRoute, private router: Router) { }

  ngOnInit() {
    this.loginForm = this.formBuilder.group({
      username: [],
      room: []
    })
  }

  randomFromRange(max: number): number {
    return Math.floor(Math.random() * max);
  }

  Suggest() {
    let value = this.loginForm.controls['username'].value;
    let value1 = value + this.randomFromRange(100);
    let value2;
    do {
      value2 = value + this.randomFromRange(100);
    } while (value2 == value1);
    let value3;
    do {
      value3 = value + this.randomFromRange(100);
    } while (value3 == value2);

    this.suggestions = [{ name: value1 },
      { name: value2 },
      { name: value3 }
    ]
  }

  AppendNumbers() {
    if (!this.append) {
      Math.random
      this.loginForm.controls['username'].setValue(this.loginForm.controls['username'].value + this.randomFromRange(100).toString());
      this.append = true;
    }
    else {
      this.append = false;
    }
  }

  OnSubmit() {
    this.router.navigateByUrl('/room/' + this.loginForm.controls['room'].value, { state: {username: this.loginForm.controls['username'].value }});
  }

}

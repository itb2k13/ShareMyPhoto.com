import { Component, OnInit } from '@angular/core';
import { ApiService } from '../api.service';
import { ApiResponse } from '../api-response';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {

  Input: string;
  Output: string;
  isLoadingResults;

  constructor(private api: ApiService) {
    this.Input = '';
    this.Output = '';
    this.isLoadingResults = false;
  }

  ngOnInit(): void {
  }

  public getLink() {

    this.isLoadingResults = true;

    this.api.get(this.Input)
      .subscribe((res: ApiResponse) => {
        this.Output = res?.output?.share || null;
        this.isLoadingResults = false;
      }, err => {
        this.isLoadingResults = false;
      });

  }

}

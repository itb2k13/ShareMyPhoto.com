import { Component, OnInit } from '@angular/core';
import { ApiService } from '../api.service';
import { ApiResponse } from '../api-response';
import config from '../app.config';
import { LocalStoreService } from '../local-store.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {

  Input: string;
  Output: string;
  BgImages: string[];
  BgImage: string;
  Message: string;
  isLoadingResults;

  constructor(private api: ApiService, private store: LocalStoreService) {
    this.Input = '';
    this.Output = '';
    this.isLoadingResults = false;
    this.BgImages = config.bgImages;
    this.BgImage = this.BgImages[Math.floor(Math.random() * this.BgImages.length)];
  }

  ngOnInit(): void {
  }

  public getLink() {

    this.isLoadingResults = true;

    this.api.get(this.Input)
      .subscribe((res: ApiResponse) => {
        this.Output = res?.output?.share || null;
        this.store.saveLocal(res?.output?.share);
        this.Message = res?.errorMessage;
        this.isLoadingResults = false;
      }, err => {
        this.Message = err?.error.message;
        this.isLoadingResults = false;
      });

  }

  public getHistory() {
    return this.store.getLocal().slice(0).slice(-5);
  }

}

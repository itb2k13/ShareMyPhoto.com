import { Component, OnInit } from '@angular/core';
import { ApiService } from '../api.service';
import { ApiResponse } from '../api-response';
import { LocalStoreService } from '../local-store.service';
import { AppConfigService } from '../app-config.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {

  Input: string;
  Output: string;
  BucketName: string;
  BgImages: string[];
  BgImage: string;
  Message: string;
  isLoadingResults;

  constructor(private api: ApiService, private store: LocalStoreService, private config: AppConfigService) {
    this.Input = '';
    this.Output = '';
    this.BucketName = 'ph.otos.online';
    this.isLoadingResults = false;
    this.BgImages = config.getConfig().bgImages;
    this.BgImage = this.BgImages[Math.floor(Math.random() * this.BgImages.length)];
  }

  ngOnInit(): void {
  }

  public getLink() {

    this.isLoadingResults = true;

    this.api.get(this.Input, this.BucketName)
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

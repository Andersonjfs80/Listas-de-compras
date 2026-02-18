import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';

@Component({
    selector: 'app-home-wrapper',
    templateUrl: './home-wrapper.component.html',
    styleUrls: ['./home-wrapper.component.scss'],
    standalone: false
})
export class HomeWrapperComponent implements OnInit {
    iframeUrl: SafeResourceUrl = '';

    constructor(
        private route: ActivatedRoute,
        private sanitizer: DomSanitizer
    ) { }

    ngOnInit(): void {
        const url = this.route.snapshot.data['url'] || 'http://localhost/home';
        this.iframeUrl = this.sanitizer.bypassSecurityTrustResourceUrl(url);
    }
}

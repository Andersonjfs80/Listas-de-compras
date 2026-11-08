import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';

@Component({
    selector: 'app-autenticacao-wrapper',
    templateUrl: './autenticacao-wrapper.component.html',
    styleUrls: ['./autenticacao-wrapper.component.scss'],
    standalone: false
})
export class AutenticacaoWrapperComponent implements OnInit {
    iframeUrl: SafeResourceUrl = '';

    constructor(
        private route: ActivatedRoute,
        private sanitizer: DomSanitizer
    ) { }

    ngOnInit(): void {
        const url = this.route.snapshot.data['url'] || 'http://localhost:4202/login';
        this.iframeUrl = this.sanitizer.bypassSecurityTrustResourceUrl(url);
    }
}

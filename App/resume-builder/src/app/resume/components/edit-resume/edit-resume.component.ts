import { Component, OnInit } from '@angular/core';
import { ResumeTreeNode, SectionType } from '../../../models/Resume';
import { Router } from '@angular/router';
import { ResumeService } from '../../services/resume.service';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Guid } from 'guid-typescript';
import {
  BorderStyle,
  ButtonStyle,
} from '../../../common/button/button.component';

@Component({
  selector: 'app-edit-resume',
  templateUrl: './edit-resume.component.html',
  styleUrls: ['./edit-resume.component.scss'],
})
export class EditResumeComponent implements OnInit {
  resume: ResumeTreeNode;
  loading: boolean = true;
  form: FormGroup;
  phonePattern =
    '^(\\+\\d{1,2}\\s?)?\\(?\\d{3}\\)?[\\s.-]?\\d{3}[\\s.-]?\\d{4}$';

  constructor(
    private router: Router,
    private service: ResumeService,
  ) {}

  ngOnInit(): void {
    this.service.getResume(this.router.url.split('/')[2]).subscribe((res) => {
      this.resume = res;
      this.form = new FormGroup({
        title: new FormControl(this.resume.content, [Validators.required]),
        comments: new FormControl(this.resume.comments),
        name: new FormControl(this.title().content, [Validators.required]),
        email: new FormControl(this.title().children[0].content, [
          Validators.required,
          Validators.email,
        ]),
        phone: new FormControl(this.title().children[1].content, [
          Validators.required,
          Validators.pattern(this.phonePattern),
        ]),
      });
      if (this.title().children.length > 2) {
        this.form.addControl(
          'website',
          new FormControl(this.title().children[2].content),
        );
      }

      this.resume.children.forEach((node) => {
        this.createSectionFormControls(node);
      });
      this.loading = false;
      console.log(this.resume, this.form);
    });
  }

  title(): ResumeTreeNode {
    return this.resume.children.find(
      (node) => node.sectionType === SectionType.Title,
    );
  }

  sections(): ResumeTreeNode[] {
    return this.resume.children.filter(
      (node) => node.sectionType === SectionType.Section,
    );
  }

  save(): void {
    this.service.updateResume(this.resume).subscribe((res) => console.log(res));
  }

  createSectionFormControls(node: ResumeTreeNode): void {
    this.form.addControl(
      `section${node.order}title`,
      new FormControl(this.resume.children[node.order].content, [
        Validators.required,
      ]),
    );

    this.form.controls[`section${node.order}title`].valueChanges.subscribe(
      (res) => {
        this.resume.children[node.order].content = res;
      },
    );
  }

  addWebsite(): void {
    const index = this.title().children.length;
    this.title().children.push({
      id: Guid.create(),
      content: '',
      comments: '',
      children: [],
      active: true,
      userId: this.resume.userId,
      sectionType: SectionType.ListItem,
      parentId: this.title().id,
      order: index,
      depth: 2,
    });

    this.form.addControl(
      'website',
      new FormControl(this.title().children[index].content),
    );

    this.form.controls['website'].valueChanges.subscribe((res) => {
      this.title().children[index].content = res;
    });
  }

  removeWebsite(): void {
    const nodeToDrop = this.title().children.pop();
    this.form.removeControl('website');

    this.service.deleteNode(nodeToDrop).subscribe();
  }

  addSection(): void {
    const index = this.resume.children.length;

    this.resume.children.push({
      id: Guid.create(),
      content: 'Section Title',
      comments: '',
      children: [],
      active: true,
      userId: this.resume.userId,
      sectionType: SectionType.Section,
      parentId: this.resume.id,
      order: index,
      depth: 1,
    });

    this.form.addControl(
      `section${index}title`,
      new FormControl(this.resume.children[index].content, [
        Validators.required,
      ]),
    );

    this.form.controls[`section${index}title`].valueChanges.subscribe((res) => {
      this.resume.children[index].content = res;
    });
  }

  protected readonly ButtonStyle = ButtonStyle;
  protected readonly BorderStyle = BorderStyle;
}

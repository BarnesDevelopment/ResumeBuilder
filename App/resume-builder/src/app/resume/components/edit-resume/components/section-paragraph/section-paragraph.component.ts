import { Component, Input, OnChanges } from '@angular/core';
import { ResumeTreeNode } from '../../../../../models/Resume';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { UpsertSignal } from '../upsert-signal/upsert-signal';

@Component({
  selector: 'app-section-paragraph',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './section-paragraph.component.html',
  styleUrl: './section-paragraph.component.scss',
})
export class SectionParagraphComponent
  extends UpsertSignal
  implements OnChanges
{
  @Input() node: ResumeTreeNode;

  form: FormGroup;

  ngOnChanges() {
    this.form = new FormGroup({
      paragraph: new FormControl(this.node.content),
    });

    this.form.valueChanges.subscribe(value => {
      this.node.content = value.paragraph;
      this.onSave.emit(this.node);
    });
  }
}

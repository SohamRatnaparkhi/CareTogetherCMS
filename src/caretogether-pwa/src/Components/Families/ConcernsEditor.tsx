import { Grid, InputAdornment, TextField } from '@mui/material';
import { useDirectoryModel } from '../../Model/DirectoryModel';
import { useInlineEditor } from '../../useInlineEditor';
import { PersonEditorProps } from "./PersonEditorProps";
import WarningIcon from '@mui/icons-material/Warning';

export function ConcernsEditor({ familyId, person }: PersonEditorProps) {
  const directoryModel = useDirectoryModel();

  const editor = useInlineEditor(async concerns =>
    await directoryModel.updatePersonConcerns(familyId!, person.id!,
      concerns != null && concerns.length > 0 ? concerns : null),
    typeof person.concerns === 'undefined' ? null : person.concerns);

  return (
    <Grid container spacing={2}>
      {editor.editing
        ? <>
            <Grid item xs={12}>
              <TextField
                id="concerns"
                label="Concerns" placeholder="Note any safety risks, allergies, etc."
                multiline fullWidth variant="outlined" minRows={2} maxRows={5} size="small"
                InputProps={{
                  startAdornment: (
                    <InputAdornment position="start">
                      <WarningIcon />
                    </InputAdornment>
                  ),
                }}
                value={editor.value == null ? "" : editor.value}
                onChange={e => editor.setValue(e.target.value)} />
            </Grid>
            <Grid item xs={12}>
              {editor.cancelButton}
              {editor.saveButton}
            </Grid>
          </>
        : <Grid item xs={12}>
            Concerns: {person.concerns}
            {editor.editButton}
        </Grid>}
    </Grid>
  );
}

import { Button } from "@mui/material";
import EditIcon from '@mui/icons-material/Edit';
import SaveIcon from '@mui/icons-material/Save';
import UndoIcon from '@mui/icons-material/Undo';
import { useState } from "react";
import { useBackdrop } from "./useBackdrop";

export function useInlineEditor<T, U>(onSave: (value: T) => Promise<U>, savedValue?: T,
  validate?: (value?: T) => boolean) {
  const withBackdrop = useBackdrop();

  const [editing, setEditing] = useState(false);
  const [value, setValue] = useState(savedValue);

  async function saveChanges() {
    await withBackdrop(async () => {
      await onSave(value as T);
      setEditing(false);
    });
  }
  function cancelEditing() {
    setEditing(false);
    setValue(savedValue);
  }

  return {
    value,
    setValue,
    editing,
    editButton: !editing &&
      <Button
        onClick={() => setEditing(true)}
        variant="text"
        size="small"
        startIcon={<EditIcon />}
        sx={{margin: 1}}>
        Edit
      </Button>,
    cancelButton: editing &&
      <Button
        onClick={() => cancelEditing()}
        variant="contained"
        size="small"
        startIcon={<UndoIcon />}
        color="secondary"
        sx={{margin: 1}}>
        Cancel
      </Button>,
    saveButton: editing &&
      <Button
        disabled={value === savedValue ||
          typeof(value) === 'undefined' ||
          (typeof(validate) !== 'undefined' && !validate(value))}
        onClick={saveChanges}
        variant="contained"
        size="small"
        startIcon={<SaveIcon />}
        sx={{margin: 1}}>
        Save
      </Button>
  }
}

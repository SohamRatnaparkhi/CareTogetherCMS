import { useState } from 'react';
import makeStyles from '@mui/styles/makeStyles';
import { Button, Dialog, DialogActions, DialogContent, DialogTitle, FormControl, Grid, InputLabel, MenuItem, Select } from '@mui/material';
import { ArrangementPolicy, Arrangement, ArrangementFunction, RoleApprovalStatus, Person, Family } from '../../GeneratedClient';
import { visibleFamiliesData } from '../../Model/ModelLoader';
import { useRecoilValue } from 'recoil';
import { useParams } from 'react-router-dom';
import { useBackdrop } from '../../useBackdrop';
import { DialogHandle } from "../../useDialogHandle";
import { useReferralsModel } from '../../Model/ReferralsModel';
import { usePersonAndFamilyLookup } from '../../Model/DirectoryModel';

const useStyles = makeStyles((theme) => ({
  form: {
    '& .MuiFormControl-root': {
    }
  },
  ageYears: {
    width: '20ch'
  }
}));

interface AssignArrangementFunctionDialogProps {
  handle: DialogHandle
  referralId: string
  arrangement: Arrangement
  arrangementPolicy: ArrangementPolicy
  arrangementFunction: ArrangementFunction
}

export function AssignArrangementFunctionDialog({
  handle, referralId, arrangement, arrangementPolicy, arrangementFunction
}: AssignArrangementFunctionDialogProps) {
  const classes = useStyles();
  
  const familyIdMaybe = useParams<{ familyId: string }>();
  const familyId = familyIdMaybe.familyId as string;
  
  const visibleFamilies = useRecoilValue(visibleFamiliesData);

  const familyAndPersonLookup = usePersonAndFamilyLookup();

  const candidateNamedPeopleAssignees = arrangementFunction.eligiblePeople
    ? arrangementFunction.eligiblePeople.map(personId =>
      familyAndPersonLookup(personId) as { family: Family, person: Person | null }).filter(person =>
        !arrangement.individualVolunteerAssignments?.find(iva =>
          iva.arrangementFunction === arrangementFunction.functionName && iva.familyId === person.family!.id && iva.personId === person.person?.id))
    : [];
  const candidateVolunteerIndividualAssignees = arrangementFunction.eligibleIndividualVolunteerRoles
    ? visibleFamilies.flatMap(f => f.volunteerFamilyInfo?.individualVolunteers
      ? Object.entries(f.volunteerFamilyInfo?.individualVolunteers).filter(([volunteerId, _]) =>
        f.family!.adults!.find(a => a.item1!.id === volunteerId)!.item1!.active).flatMap(([volunteerId, volunteerInfo]) => volunteerInfo.individualRoleApprovals
        ? Object.entries(volunteerInfo.individualRoleApprovals).flatMap(([roleName, roleVersionApproval]) =>
          arrangementFunction.eligibleIndividualVolunteerRoles!.find(x => x === roleName) &&
          roleVersionApproval.find(rva => rva.approvalStatus === RoleApprovalStatus.Approved || rva.approvalStatus === RoleApprovalStatus.Onboarded) &&
          !arrangement.individualVolunteerAssignments?.find(iva =>
            iva.arrangementFunction === arrangementFunction.functionName && iva.familyId === f.family!.id && iva.personId === volunteerId)
          ? [{ family: f.family!, person: f.family!.adults!.find(a => a.item1!.id === volunteerId)!.item1 || null }]
          : [])
        : [])
      : [])
    : [];
  const candidateVolunteerFamilyAssignees = arrangementFunction.eligibleVolunteerFamilyRoles
    ? visibleFamilies.flatMap(f => f.volunteerFamilyInfo?.familyRoleApprovals
      ? Object.entries(f.volunteerFamilyInfo.familyRoleApprovals).flatMap(([roleName, roleVersionApproval]) =>
        arrangementFunction.eligibleVolunteerFamilyRoles!.find(x => x === roleName) &&
        roleVersionApproval.find(rva => rva.approvalStatus === RoleApprovalStatus.Approved || rva.approvalStatus === RoleApprovalStatus.Onboarded) &&
        !arrangement.familyVolunteerAssignments?.find(fva =>
          fva.arrangementFunction === arrangementFunction.functionName && fva.familyId === f.family!.id)
        ? [{ family: f.family!, person: null as Person | null }]
        : [])
      : [])
    : [];
  const allCandidateAssignees = candidateNamedPeopleAssignees.concat(candidateVolunteerFamilyAssignees).concat(candidateVolunteerIndividualAssignees);
  const deduplicatedCandidateAssignees = allCandidateAssignees.filter((item, i) =>
    allCandidateAssignees.indexOf(item) === i).sort((a, b) => {
      const aPrimaryContact = a.family!.adults!.find(adult =>
        a.family.primaryFamilyContactPersonId === adult.item1!.id)!.item1!;
      const bPrimaryContact = b.family!.adults!.find(adult =>
        b.family.primaryFamilyContactPersonId === adult.item1!.id)!.item1!;
      
      const aFirst = a.person ? a.person.firstName! : null;
      const aLast = a.person ? a.person.lastName! : aPrimaryContact.lastName!;
      const bFirst = b.person ? b.person.firstName! : null;
      const bLast = b.person ? b.person.lastName! : bPrimaryContact.lastName!;

      // Sort by last name, then by first name (if applicable)
      return aLast < bLast ? -1 : aLast > bLast ? 1 :
        aFirst == null || bFirst == null ? 0 : aFirst < bFirst ? -1 : aFirst > bFirst ? 1 : 0;
    });
  const candidateAssignees = deduplicatedCandidateAssignees.map(candidate => {
    if (candidate.person == null) {
      return {
        familyId: candidate.family.id!,
        personId: null as string | null,
        key: candidate.family.id!,
        displayName: `${candidate.family.adults!.find(adult => candidate.family.primaryFamilyContactPersonId === adult.item1?.id)?.item1!.lastName} Family`
      };
    } else {
      return {
        familyId: candidate.family.id!,
        personId: candidate.person.id! as string | null,
        key: `${candidate.family.id!}|${candidate.person.id || ''}`,
        displayName: `${candidate.person.firstName} ${candidate.person.lastName}`
      };
    }
  });
  
  const [fields, setFields] = useState({
    assigneeKey: ''
  });
  const { assigneeKey } = fields;
  
  const referralsModel = useReferralsModel();
  
  const withBackdrop = useBackdrop();

  async function save() {
    await withBackdrop(async () => {
      handle.closeDialog(); // This is placed here so values are not recalculated unnecessarily (which otherwise results in errors).
      const assigneeInfo = candidateAssignees.find(ca => ca.key === assigneeKey);
      if (assigneeInfo?.personId == null) {
        await referralsModel.assignVolunteerFamily(familyId, referralId, arrangement.id!,
          assigneeInfo!.familyId, arrangementFunction.functionName!);
      } else {
        await referralsModel.assignIndividualVolunteer(familyId, referralId, arrangement.id!,
          assigneeInfo!.familyId, assigneeInfo!.personId, arrangementFunction.functionName!);
      }
      //TODO: Error handling (start with a basic error dialog w/ request to share a screenshot, and App Insights logging)
    });
  }

  return (
    <Dialog open={handle.open} onClose={handle.closeDialog} key={handle.key}
      scroll='body' aria-labelledby="assign-volunteer-title">
      <DialogTitle id="assign-volunteer-title">
        Assign {arrangementFunction.functionName}
      </DialogTitle>
      <DialogContent>
        <form className={classes.form} noValidate autoComplete="off">
          <Grid container spacing={2}>
            <Grid item xs={12}>
              <FormControl required fullWidth size="small" sx={{marginTop: 1}}>
                <InputLabel id="assignee-label">Assignee</InputLabel>
                <Select
                  labelId="assignee-label" id="assignee"
                  value={assigneeKey}
                  onChange={e => setFields({...fields, assigneeKey: e.target.value as string})}>
                    <MenuItem key="placeholder" value="" disabled>
                      Select a family or individual to assign
                    </MenuItem>
                    {candidateAssignees.map(candidate =>
                      <MenuItem key={candidate.key} value={candidate.key}>{candidate.displayName}</MenuItem>)}
                </Select>
              </FormControl>
            </Grid>
          </Grid>
        </form>
      </DialogContent>
      <DialogActions>
        <Button onClick={handle.closeDialog} color="secondary">
          Cancel
        </Button>
        <Button onClick={save} variant="contained" color="primary"
          disabled={assigneeKey?.length === 0}>
          Assign
        </Button>
      </DialogActions>
    </Dialog>
  );
}

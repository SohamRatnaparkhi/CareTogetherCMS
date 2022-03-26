import { TableCell, TableRow } from "@mui/material";
import { useRecoilValue } from "recoil";
import { Arrangement, ArrangementFunction, ArrangementPolicy, FamilyVolunteerAssignment, FunctionRequirement, IndividualVolunteerAssignment } from "../../GeneratedClient";
import { policyData } from "../../Model/ConfigurationModel";
import { useFamilyLookup, usePersonLookup } from "../../Model/DirectoryModel";
import { usePermissions } from "../../Model/SessionModel";
import { useDialogHandle } from "../../useDialogHandle";
import { FamilyName } from "../Families/FamilyName";
import { PersonName } from "../Families/PersonName";
import { IconRow } from "../IconRow";
import { AssignArrangementFunctionDialog } from "./AssignArrangementFunctionDialog";

type ArrangementFunctionRowProps = {
  partneringFamilyId: string
  referralId: string
  arrangement: Arrangement
  arrangementPolicy: ArrangementPolicy
  functionPolicy: ArrangementFunction
};

export function ArrangementFunctionRow({
  partneringFamilyId, referralId, arrangement, arrangementPolicy, functionPolicy
}: ArrangementFunctionRowProps) {
  const policy = useRecoilValue(policyData);
  const permissions = usePermissions();
  const familyLookup = useFamilyLookup();
  const personLookup = usePersonLookup();
  
  const addAssignmentDialogHandle = useDialogHandle();
  const removeAssignmentDialogHandle = useDialogHandle();

  const canComplete = true; //TODO: Implement permissions!
  
  const assignments = (arrangement.familyVolunteerAssignments || [] as Array<FamilyVolunteerAssignment | IndividualVolunteerAssignment>).concat(
    arrangement.individualVolunteerAssignments || []).filter(assignment =>
    assignment.arrangementFunction === functionPolicy.functionName) as Array<FamilyVolunteerAssignment | IndividualVolunteerAssignment>;
  
  const isMissing =
    !arrangement.familyVolunteerAssignments?.some(x => x.arrangementFunction === functionPolicy.functionName) &&
    !arrangement.individualVolunteerAssignments?.some(x => x.arrangementFunction === functionPolicy.functionName);

  return (
    <>
      <TableRow key={functionPolicy.functionName}>
        <TableCell sx={{ padding: 0 }} colSpan={assignments.length === 0 ? 2 : 1}>
          <IconRow icon={isMissing
            ? functionPolicy.requirement === FunctionRequirement.ZeroOrMore ? "⚠" : "❌"
            : "✅"}
            onClick={canComplete ? addAssignmentDialogHandle.openDialog : undefined}>
            {functionPolicy.functionName}
          </IconRow>
        </TableCell>
        <TableCell sx={{ padding: 0 }}>
          {assignments.map(assignment =>
            <IconRow key={JSON.stringify(assignment)} icon=''
              onClick={canComplete ? removeAssignmentDialogHandle.openDialog : undefined}>
              {assignment instanceof FamilyVolunteerAssignment &&
                <FamilyName family={familyLookup(assignment.familyId)} />}
              {assignment instanceof IndividualVolunteerAssignment &&
                <PersonName person={personLookup(assignment.familyId, assignment.personId)} />}
            </IconRow>)}
        </TableCell>
      </TableRow>
      {addAssignmentDialogHandle.open && <AssignArrangementFunctionDialog handle={addAssignmentDialogHandle}
        referralId={referralId}
        arrangement={arrangement}
        arrangementPolicy={arrangementPolicy}
        arrangementFunction={functionPolicy} />}
      {/* {removeAssignmentDialogHandle.open && <UnassignArrangementFunctionDialog handle={removeAssignmentDialogHandle}
        referralId={referralId}
        arrangement={arrangement}
        arrangementPolicy={arrangementPolicy}
        arrangementFunction={functionPolicy}
        assignment={assignment} />} */}
    </>
  );
}
